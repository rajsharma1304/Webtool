using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SICT.BusinessLayer.V1;
using NUnit;
using NUnit_Application;
using NUnit.Framework;
using SICT.DataContracts;
using System.Data;
using System.Net;
using System.Configuration;
using SICT.DataAccessLayer;
using SICT.Constants;

namespace NUnit.Test
{
    public class NUnitTestClass
    {
        
         static FormDetails TempFormDetails = new FormDetails();
         static Airline[] Airlines = new Airline[1];
         static Language[] Languages = new Language[1];
         static string SessionId = "abc";

        

        [TestFixture]
        public class SampleTestClass
        {
            [SetUp]
            public void  Assignment()
            {
                        Languages[0].LanguageId = 1;
                        Languages[0].FirstSerialNo = 10;
                        Languages[0].LastSerialNo = 20;

                        Airlines[0].AirlineId = 1;
                        Airlines[0].FlightNumber = "1";
                        Airlines[0].DestinationId = 1;
                        Airlines[0].BCardsDistributed = 1;
                        Airlines[0].Languages = Languages;

                        TempFormDetails.Airlines = Airlines;
                        TempFormDetails.IsDepartureForm = true;
                        TempFormDetails.AirportId=1;
                        TempFormDetails.FieldWorkDate="2015-1-13";
                        TempFormDetails.InterviewerId = 1;
            }

            [TestCase]
            public void AddTest()
            {
                UserDetailsBusiness ObjSample = new UserDetailsBusiness();
                int result = ObjSample.Add(20, 10);
                Assert.AreEqual(40, result);
            }

            [TestCase]
            public void SubtractTest()
            {
                UserDetailsBusiness ObjSample = new UserDetailsBusiness();
                int result = ObjSample.Subtract(20, 10);
                Assert.AreEqual(10, result);
            }

            [TestCase]
            public void Multiply()
            {
                UserDetailsBusiness ObjSample = new UserDetailsBusiness();
                int result = ObjSample.Multiply(10, 10);
                Assert.AreEqual(10, result);
            }

            [TestCase]
            public void Login()
            {
                UserDetailsBusiness ObjSample = new UserDetailsBusiness();
                LoginInformation LoginInformatoin = ObjSample.CompareHashAndAuthenticate("1f9f45d5360619e977ceace59790d48d");
                Assert.AreEqual(LoginInformatoin.IsValidUser, true);
            }


            //[TestCase]
            //public void DepartureFormEntry()
            //{
            //    DepartureFormBusiness ObjDepartureFormBusiness = new DepartureFormBusiness();
            //    ReturnValue ReturnValue = ObjDepartureFormBusiness.SetFormDetails(ref TempFormDetails, SessionId);
            //    Assert.AreEqual(ReturnValue.ReturnCode, 1);
            //}
        }
    }
}
