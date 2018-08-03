using System;

namespace MindsetDailyAutoUpload
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblplswt = new System.Windows.Forms.Label();
            this.lblcomsuc = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblINTLFile = new System.Windows.Forms.Label();
            this.lblEuropefile = new System.Windows.Forms.Label();
            this.lblboeingfile = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblConsolidateINTL = new System.Windows.Forms.Label();
            this.lblConsolidateEurope = new System.Windows.Forms.Label();
            this.lblConsolidateBoeing = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(38, 58);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(318, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(281, 133);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblplswt
            // 
            this.lblplswt.AutoSize = true;
            this.lblplswt.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblplswt.ForeColor = System.Drawing.Color.Maroon;
            this.lblplswt.Location = new System.Drawing.Point(157, 25);
            this.lblplswt.Name = "lblplswt";
            this.lblplswt.Size = new System.Drawing.Size(99, 17);
            this.lblplswt.TabIndex = 3;
            this.lblplswt.Text = "Please Wait...";
            this.lblplswt.Visible = false;
            // 
            // lblcomsuc
            // 
            this.lblcomsuc.AutoSize = true;
            this.lblcomsuc.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcomsuc.ForeColor = System.Drawing.Color.Maroon;
            this.lblcomsuc.Location = new System.Drawing.Point(116, 97);
            this.lblcomsuc.Name = "lblcomsuc";
            this.lblcomsuc.Size = new System.Drawing.Size(167, 17);
            this.lblcomsuc.TabIndex = 4;
            this.lblcomsuc.Text = "Completed Successfully";
            this.lblcomsuc.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(119, 133);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(156, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Export Combined Data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Exported Files Path:";
            // 
            // lblINTLFile
            // 
            this.lblINTLFile.AutoSize = true;
            this.lblINTLFile.Location = new System.Drawing.Point(35, 199);
            this.lblINTLFile.Name = "lblINTLFile";
            this.lblINTLFile.Size = new System.Drawing.Size(0, 13);
            this.lblINTLFile.TabIndex = 7;
            // 
            // lblEuropefile
            // 
            this.lblEuropefile.AutoSize = true;
            this.lblEuropefile.Location = new System.Drawing.Point(35, 212);
            this.lblEuropefile.Name = "lblEuropefile";
            this.lblEuropefile.Size = new System.Drawing.Size(0, 13);
            this.lblEuropefile.TabIndex = 8;
            // 
            // lblboeingfile
            // 
            this.lblboeingfile.AutoSize = true;
            this.lblboeingfile.Location = new System.Drawing.Point(35, 225);
            this.lblboeingfile.Name = "lblboeingfile";
            this.lblboeingfile.Size = new System.Drawing.Size(0, 13);
            this.lblboeingfile.TabIndex = 9;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MindsetDailyAutoUpload.Properties.Resources.loading;
            this.pictureBox1.Location = new System.Drawing.Point(373, 124);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 41);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(373, 51);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(30, 30);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // lblConsolidateINTL
            // 
            this.lblConsolidateINTL.AutoSize = true;
            this.lblConsolidateINTL.Location = new System.Drawing.Point(38, 242);
            this.lblConsolidateINTL.Name = "lblConsolidateINTL";
            this.lblConsolidateINTL.Size = new System.Drawing.Size(0, 13);
            this.lblConsolidateINTL.TabIndex = 12;
            // 
            // lblConsolidateEurope
            // 
            this.lblConsolidateEurope.AutoSize = true;
            this.lblConsolidateEurope.Location = new System.Drawing.Point(38, 255);
            this.lblConsolidateEurope.Name = "lblConsolidateEurope";
            this.lblConsolidateEurope.Size = new System.Drawing.Size(0, 13);
            this.lblConsolidateEurope.TabIndex = 13;
            // 
            // lblConsolidateBoeing
            // 
            this.lblConsolidateBoeing.AutoSize = true;
            this.lblConsolidateBoeing.Location = new System.Drawing.Point(38, 268);
            this.lblConsolidateBoeing.Name = "lblConsolidateBoeing";
            this.lblConsolidateBoeing.Size = new System.Drawing.Size(0, 13);
            this.lblConsolidateBoeing.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 318);
            this.Controls.Add(this.lblConsolidateBoeing);
            this.Controls.Add(this.lblConsolidateEurope);
            this.Controls.Add(this.lblConsolidateINTL);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblboeingfile);
            this.Controls.Add(this.lblEuropefile);
            this.Controls.Add(this.lblINTLFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lblcomsuc);
            this.Controls.Add(this.lblplswt);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Name = "Form1";
            this.Text = "MindsetDailyAutoUpload";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblplswt;
        private System.Windows.Forms.Label lblcomsuc;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblINTLFile;
        private System.Windows.Forms.Label lblEuropefile;
        private System.Windows.Forms.Label lblboeingfile;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblConsolidateINTL;
        private System.Windows.Forms.Label lblConsolidateEurope;
        private System.Windows.Forms.Label lblConsolidateBoeing;
    }
}

