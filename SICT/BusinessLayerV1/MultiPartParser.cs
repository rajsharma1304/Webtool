using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SICT.BusinessLayer.V1
{
    public class MultiPartParser
    {
        private static readonly string CLASS_NAME = "MultipartParser";

        public bool Success
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        public byte[] FileContents
        {
            get;
            private set;
        }

        public MultiPartParser(System.IO.Stream Stream)
        {
            this.Parse(Stream, System.Text.Encoding.UTF8);
        }

        public MultiPartParser(System.IO.Stream Stream, System.Text.Encoding Encoding)
        {
            this.Parse(Stream, Encoding);
        }

        private void Parse(System.IO.Stream Stream, System.Text.Encoding Encoding)
        {
            try
            {
                this.Success = false;
                SICTLogger.WriteVerbose(MultiPartParser.CLASS_NAME, "Parse ", "Inside Multiparser Class Parse method, Reading stream .");
                byte[] data = this.ToByteArray(Stream);
                string content = Encoding.GetString(data);
                SICTLogger.WriteVerbose(MultiPartParser.CLASS_NAME, "Parse ", "GetString method complete Multiparser Class");
                int delimiterEndIndex = content.IndexOf("\r\n");
                if (delimiterEndIndex > -1)
                {
                    string delimiter = content.Substring(0, content.IndexOf("\r\n"));
                    SICTLogger.WriteVerbose(MultiPartParser.CLASS_NAME, "Parse ", "contentTypeMatch Multiparser Class");
                    Regex re = new Regex("(?<=Content\\-Type:)(.*?)(?=\\r\\n\\r\\n)");
                    Match contentTypeMatch = re.Match(content);
                    SICTLogger.WriteVerbose(MultiPartParser.CLASS_NAME, "Parse ", "contentTypeMatch Multiparser Class");
                    re = new Regex("(?<=filename\\=\\\")(.*?)(?=\\\")");
                    Match filenameMatch = re.Match(content);
                    SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "filenameMatch Multiparser Class");
                    if (contentTypeMatch.Success && filenameMatch.Success)
                    {
                        this.ContentType = contentTypeMatch.Value.Trim();
                        this.Filename = filenameMatch.Value.Trim();
                        SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "FileName Multiparser Class" + this.Filename);
                        int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                        SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "StartIndex Multiparser Class" + startIndex);
                        byte[] delimiterBytes = Encoding.GetBytes("\r\n" + delimiter);
                        int endIndex = this.IndexOf(data, delimiterBytes, startIndex);
                        SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "EndIndex Multiparser Class" + endIndex);
                        int contentLength = endIndex - startIndex;
                        byte[] fileData = new byte[contentLength];
                        System.Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);
                        SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "Buffer.BlockCopy Multiparser Class");
                        this.FileContents = fileData;
                        this.Success = true;
                    }
                    else
                    {
                        SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "condition contentTypeMatch.Success && filenameMatch.Success did not match");
                    }
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(MultiPartParser.CLASS_NAME, "Parse ", Ex);
            }
        }

        private int IndexOf(byte[] SearchWithin, byte[] SerachFor, int StartIndex)
        {
            int Index = 0;
            int StartPos = System.Array.IndexOf<byte>(SearchWithin, SerachFor[0], StartIndex);
            SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "IndexOf ", "Start of IndexOf");
            int result;
            if (StartPos != -1)
            {
                while (StartPos + Index < SearchWithin.Length)
                {
                    if (SearchWithin[StartPos + Index] == SerachFor[Index])
                    {
                        Index++;
                        if (Index == SerachFor.Length)
                        {
                            result = StartPos;
                            return result;
                        }
                    }
                    else
                    {
                        StartPos = System.Array.IndexOf<byte>(SearchWithin, SerachFor[0], StartPos + Index);
                        if (StartPos == -1)
                        {
                            result = -1;
                            return result;
                        }
                        Index = 0;
                    }
                }
            }
            SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "IndexOf ", "End of IndexOf");
            result = -1;
            return result;
        }

        private byte[] ToByteArray(System.IO.Stream Stream)
        {
            SICTLogger.WriteInfo(MultiPartParser.CLASS_NAME, "Parse ", "Inside Multiparser Class Parse method, Converting to byte array");
            byte[] result;
            try
            {
                byte[] Buffer = new byte[2097152];
                using (System.IO.MemoryStream Ms = new System.IO.MemoryStream())
                {
                    while (true)
                    {
                        int read = Stream.Read(Buffer, 0, Buffer.Length);
                        if (read <= 0)
                        {
                            break;
                        }
                        Ms.Write(Buffer, 0, read);
                    }
                    result = Ms.ToArray();
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(MultiPartParser.CLASS_NAME, "Parse ", Ex);
                result = null;
            }
            return result;
        }
    }
}
