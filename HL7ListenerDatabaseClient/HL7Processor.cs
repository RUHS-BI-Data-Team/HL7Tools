// process new patient (by HL7 event)

// update existing patient (A08)

// Patient Visit Status = admitted, registered, etc


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

using NHapi.Base;
using NHapi.Base.Log;

//contains all HL7 Events!!! YES!!!
using NHapi.Model.V251.Message;

using NHapi.Model.V251.Group;
using NHapi.Model.V251.Segment;
using NHapi.Model.V251.Datatype;
using NHapi.Base.Parser;
using NHapi.Base.Model;

using NHapi.Base.Util;

namespace HL7ListenerDatabase
{
    class HL7Processor : IDisposable
    {

        // Globals!
        SqlConnection oConn; // = new SqlConnection(HL7Listener.Properties.Settings.Default.connectionStr);
        SqlCommand oComm; // = new SqlCommand();

        private bool bNAK = false;


        // place this in error logger later
        string sSQLErrorDir = @"C:\HL7Listener_Logs\";
        string sSQLErrorFile = "SQL_Errors_" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".txt";


        /// <summary>
        /// Constructor
        /// </summary>
        public HL7Processor()
        {
          
            
            //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["RUHSDS_ADT.Properties.Settings.connectionStr"];

            // If found, return the connection string.
            //if (settings != null) returnValue = settings.ConnectionString;


            oConn = new SqlConnection(ConfigurationManager.ConnectionStrings["HL7DatabaseConnection"].ConnectionString);

            //oConn.ConnectionString = (System.Configuration.ConfigurationManager.AppSettings["RUHSDS_ADT.Properties.Settings.connectionStr"]);
            oComm = new SqlCommand();
            oComm.Connection = oConn;
            oComm.CommandType = CommandType.StoredProcedure;
            oComm.CommandText = "uspInsertHL7data";
            oComm.Parameters.Add("@HL7Data", SqlDbType.NVarChar,-1);
            oComm.Parameters.Add("@MessageType", SqlDbType.NVarChar,3);

        }


        //private void GetHL7Events() //only these HL7 Events will be processed
        //{
        //    // manually fill array (use SQL SP Later)

        //    //string[] sHL7Events;
        //    /*
        //    sHL7Events[0] = "A01";
        //    sHL7Events[1] = "A04";
        //    sHL7Events[2] = "A08";
        //    */

        //}


        public bool sendNAK
        {
            get
            {
                return bNAK;
            }
            set //this may or may not ever be used directly in this fashion
            {
                bNAK = value;
            }

        }

        private void ProcessHL7Event(HL7Message message)
        {

            //GetHL7Events();

            string sMessageHL7Event = message.GetHL7Item("MSH-9.2")[0];

            // create HL7Processor to update SQL DB with HL7! <ala: Mike B>
            //HL7Processor hl7Processor = new HL7Processor();
            //hl7Processor.ProcessData(message);

            //string sError = "Error Processing Transaction " + message.GetHL7Item("MSH-9")[0];

            //ErrorLogger oErrorLogger = new ErrorLogger();
            //oErrorLogger.ProcessError(sError);






                //int iEventTypeIndex = Array.IndexOf(sHL7Events, sMessageHL7Event);

                // Console.WriteLine("Event Type index = {0}",iEventTypeIndex.ToString());

                /*
                if (iEventTypeIndex = -1) // -1 == array item not found
                {

                }
                 */

                Console.WriteLine("Ready to process event: {0}", sMessageHL7Event);   



            //ProcessPatient(message);

        }
        
        public void ProcessData (HL7Message message)
        {
            //Console.WriteLine("processing message!");

            //string sSex = null;

            // SQL SP Writer
            //ProcessHL7Event(message);

            

            Console.WriteLine("============================================");
            Console.WriteLine("Patient First Name = {0}", message.GetHL7Item("PID-5.2")[0]);
            Console.WriteLine("Patient Last Name = {0}", message.GetHL7Item("PID-5.1")[0]);
            Console.WriteLine("Patient Med Rec # = {0}", message.GetHL7Item("PID-3.1")[0]);

            //Console.WriteLine("Patient Sex = {0}", sSex);

            


            Console.WriteLine("Patient Sex = {0}", message.GetHL7Item("PID-8")[0]);
            Console.WriteLine("Patient Race = {0}", message.GetHL7Item("PID-10")[0]);
            Console.WriteLine("Patient Street Address = {0}", message.GetHL7Item("PID-11.1")[0]);
            Console.WriteLine("Patient City = {0}", message.GetHL7Item("PID-11.3")[0]);
            Console.WriteLine("Patient State = {0}", message.GetHL7Item("PID-11.4")[0]);
            Console.WriteLine("Patient Zip = {0}", message.GetHL7Item("PID-11.5")[0]);
            Console.WriteLine("Patient Country = {0}", message.GetHL7Item("PID-11.6")[0]);
            Console.WriteLine("Patient Class = {0}", message.GetHL7Item("PV1-2")[0]);
            
            Console.WriteLine("============================================");


        }

        public void ProcessHL7Data(string sHL7) //nHAPI ver
        {
            //ParseMessage(sHL7);

            //use switch later
            ProcessADT(sHL7);

            //switch (GetHL7Event(sHL7))
            //{
            //    case "A01": 
            //    case "A02": 
            //    case "A03": 
            //    case "A04": 
            //    case "A08":
            //        ProcessADT(sHL7);
            //        break;
            //} 

            /*
            if (GetHL7Event(sHL7) == "A08")
            {
                ProcessADT(sHL7);
            }

            if (GetHL7Event(sHL7) == "A04")
            {
                ProcessADT(sHL7);
            }

            if (GetHL7Event(sHL7) == "A01")
            {
                ProcessADT(sHL7);
            }

            if (GetHL7Event(sHL7) == "A02")
            {
                ProcessADT(sHL7);
            }

            if (GetHL7Event(sHL7) == "A03")
            {
                ProcessADT(sHL7);
            }
             */

            // process all ADT!!!
           // ProcessADT(sHL7);

        }

        public string ConvertHL7Date(string sDate, Boolean IncludeTime = true)
        {
            // Input = CCYYMMDDTTTT
            string sMonth = sDate.Substring(4,2);
            string sDay = sDate.Substring(6, 2);
            string sYear = sDate.Substring(0, 4);
            string sHours = "00";
            string sMins = "00";
            string sSecs = "00";
            if (IncludeTime)
            {
                sHours = sDate.Substring(8, 2);
                sMins = sDate.Substring(10, 2);
                sSecs = sDate.Substring(12, 2);
            }
            return sMonth + "/" + sDay + "/" + sYear + " " + sHours + ":" + sMins + ":" + sSecs;
        }


        


        public void LogSQLError (string sError, string sHL7)
        {



            //check to see if file exists and create a file if not

            string sHeader = "=======================================================" + System.Environment.NewLine;
            sHeader += "SQL Error Occurred: " + string.Format("{0:yyyy-MM-dd hh:mm:ss tt}", DateTime.Now) + System.Environment.NewLine;
            sHeader += "=======================================================" + System.Environment.NewLine;
            //string sFooter = "=======================================================" + System.Environment.NewLine;

            bNAK = true; //send NAK

            // string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);

            if (!Directory.Exists(sSQLErrorDir))
                Directory.CreateDirectory(sSQLErrorDir);
            
            using (StreamWriter writetext = new StreamWriter(sSQLErrorDir + sSQLErrorFile, true))
            {
                writetext.WriteLine(sHeader);
                writetext.WriteLine(sError + System.Environment.NewLine);
                writetext.WriteLine(sHL7);
                //writetext.WriteLine(sFooter);
            }

        }


        public void ProcessADT(string sHL7)
        {
            var parser = new NHapi.Base.Parser.PipeParser();
            var parsedMessage = parser.Parse(sHL7);
            
            //Terse it!
            Terser terser = new Terser(parsedMessage);
            if (terser.Get("/MSH-9-1").ToString() == "ADT")
            {
                oComm.Parameters["@HL7Data"].Value = sHL7;
                oComm.Parameters["@MessageType"].Value = "ADT";
                try
                {

                    if (oConn.State != ConnectionState.Open)
                    {
                        oConn.Open();
                    }
                    oComm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogSQLError(ex.ToString(), sHL7);
                    Console.WriteLine("SQL Error Occurred and was logged");                    
                    bNAK = true;//send NAK
                }
                finally
                {
                    
                }

            }
                
            //Grab RequiredFields

            ///PID-11(0)-1 = grabs first repetition of PID11
            ///PID-11(1)-1 = grabs second repetition of PID11

            //string sEventType = terser.Get("/MSH-9-1");
            //string sMessageType = terser.Get("/MSH-9-2");
            //string sSendingFac = terser.Get("/MSH-4");
            //string sControlID = terser.Get("/MSH-10");
            //string sHL7DateTime = ConvertHL7Date(terser.Get("/MSH-7"));
            //DateTime dtHL7Message = Convert.ToDateTime(sHL7DateTime);
            //string sEventDateTime = ConvertHL7Date(terser.Get("/EVN-2"));
            //DateTime dtEventDateTime = Convert.ToDateTime(sEventDateTime);
            //string sEventReason = terser.Get("/EVN-4");
            //string sOperatorID = terser.Get("/EVN-5-1");
            //string sOperatorFName = terser.Get("/EVN-5-3");
            //string sOperatorLName = terser.Get("/EVN-5-2");
            //string sOperatorAssigningAuth = terser.Get("/EVN-5-9");
            //string sOperatorAssigningFac = terser.Get("/EVN-5-14");
            //string sMedRecNumber = terser.Get("/PID-3");
            //string sPtFName = terser.Get("/PID-5-2");
            //string sPtLName = terser.Get("/PID-5-1");
            //string sPtMName = terser.Get("/PID-5-3");
            //string sConvertDOB = ConvertHL7Date(terser.Get("/PID-7"),false);
            //DateTime dtDOB = Convert.ToDateTime(sConvertDOB);
            //string sSex = terser.Get("/PID-8");
            //string sRace = terser.Get("/PID-10");
            //string sStreetAddress = terser.Get("/PID-11-1");
            //string sCity = terser.Get("/PID-11-3");
            //string sState = terser.Get("/PID-11-4");
            //string sZip = terser.Get("/PID-11-5");
            //string sCountry = terser.Get("/PID-11-6");
            //string sVisitNumber = terser.Get("/PID-18-1");
            //string sPtClass = terser.Get("/PV1-2");
            //string sPtUnit = terser.Get("/PV1-3-1");
            //string sPtRoom = terser.Get("/PV1-3-2");
            //string sPtBed = terser.Get("/PV1-3-3");
            //string sHospSvc = terser.Get("/PV1-10");


            ////Console.WriteLine("Date = {0}", sHL7DateTime);

            
            //Console.WriteLine("Event Type = {0}", sEventType);
            //Console.WriteLine("Message Type = {0}", sMessageType);
            //Console.WriteLine("ControlID = {0}", sControlID);
            //Console.WriteLine("MedRecNumber = {0}", sMedRecNumber);
            //Console.WriteLine("VisitNumber = {0}", sVisitNumber);
            //Console.WriteLine("Patient First Name = {0}", sPtFName);
            //Console.WriteLine("Patient Last Name = {0}", sPtLName);
            //Console.WriteLine("Patient Middle Name = {0}", sPtMName);
            //Console.WriteLine("Patient DOB = {0}", sConvertDOB);
            //Console.WriteLine("Sex = {0}", sSex);
            //Console.WriteLine("Race = {0}", sRace);
            //Console.WriteLine("Street = {0}", sStreetAddress);
            //Console.WriteLine("City = {0}", sCity);
            //Console.WriteLine("State = {0}", sState);
            //Console.WriteLine("Zip = {0}", sZip);
            //Console.WriteLine("Country = {0}", sCountry);
            //Console.WriteLine("Patient Class = {0}", sPtClass);
            //Console.WriteLine("Unit = {0}", sPtUnit);
            //Console.WriteLine("Room = {0}", sPtRoom);
            //Console.WriteLine("Bed = {0}", sPtBed);
            //Console.WriteLine("Event DT = {0}", sEventDateTime);
            //Console.WriteLine("Message DT = {0}", sHL7DateTime);
            //Console.WriteLine("OperID = {0}", sOperatorID);
            //Console.WriteLine("OperFName = {0}", sOperatorFName);
            //Console.WriteLine("OperLName = {0}", sOperatorLName);
            //Console.WriteLine("OperAuth = {0}", sOperatorAssigningAuth);
            //Console.WriteLine("OperFac = {0}", sOperatorAssigningFac);

            // insert into SQL

            // As per my supervisors coding direction heheh
            //SqlConnection oConn = new SqlConnection(HL7Listener.Properties.Settings.Default.connectionStr);
            //SqlCommand oComm = new SqlCommand();
            //oComm.CommandType = CommandType.StoredProcedure;
            //oComm.CommandText = "uspInsertADTdata";
            ////
            
            //oComm.Parameters.Add("@MessageType", SqlDbType.NVarChar).Value = sMessageType;
            //oComm.Parameters.Add("@EventType", SqlDbType.NVarChar).Value = sEventType;
            //oComm.Parameters.Add("@DateOfMessage", SqlDbType.DateTime).Value = dtHL7Message;
            //oComm.Parameters.Add("@EventDate", SqlDbType.DateTime).Value = dtEventDateTime;
            //oComm.Parameters.Add("@SendingFacility", SqlDbType.NVarChar).Value = sSendingFac;

            //oComm.Parameters.Add("@EventReason", SqlDbType.NVarChar).Value = sEventReason;
            //oComm.Parameters.Add("@OperatorId", SqlDbType.NVarChar).Value = sOperatorID;
            //oComm.Parameters.Add("@OperatorFirstName", SqlDbType.NVarChar).Value = sOperatorFName;
            //oComm.Parameters.Add("@OperatorLastName", SqlDbType.NVarChar).Value = sOperatorLName;
            //oComm.Parameters.Add("@OperatorAuthority", SqlDbType.NVarChar).Value = sOperatorAssigningAuth;
            //oComm.Parameters.Add("@OperatorFacility", SqlDbType.NVarChar).Value = sOperatorAssigningFac;
            
            //oComm.Parameters.Add("@MedicalRecordNumber", SqlDbType.NVarChar).Value = sMedRecNumber;
            //oComm.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = sPtFName;
            //oComm.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = sPtLName;
            //oComm.Parameters.Add("@MiddleName", SqlDbType.NVarChar).Value = sPtMName;
            //oComm.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = dtDOB;
            //oComm.Parameters.Add("@Sex", SqlDbType.NVarChar).Value = sSex;
            //oComm.Parameters.Add("@Race", SqlDbType.NVarChar).Value = sRace;
            //oComm.Parameters.Add("@StreetAddress", SqlDbType.NVarChar).Value = sStreetAddress;
            //oComm.Parameters.Add("@City", SqlDbType.NVarChar).Value = sCity;
            //oComm.Parameters.Add("@State", SqlDbType.NVarChar).Value = sState;
            //oComm.Parameters.Add("@ZipCode", SqlDbType.NVarChar).Value = sZip;
            //oComm.Parameters.Add("@Country", SqlDbType.NVarChar).Value = "USA"; //sCountry;
            //oComm.Parameters.Add("@VisitNumber", SqlDbType.NVarChar).Value = sVisitNumber;
            //oComm.Parameters.Add("@PatientClass", SqlDbType.NVarChar).Value = sPtClass;
            //oComm.Parameters.Add("@PatientUnit", SqlDbType.NVarChar).Value = sPtUnit;
            //oComm.Parameters.Add("@PatientRoom", SqlDbType.NVarChar).Value = sPtRoom;
            //oComm.Parameters.Add("@PatientBed", SqlDbType.NVarChar).Value = sPtBed;
            //oComm.Parameters.Add("@HospitalService", SqlDbType.NVarChar).Value = sHospSvc;
            //oComm.Parameters.Add("@ControlId", SqlDbType.NVarChar).Value = sControlID;
            
            //oComm.Connection = oConn;


 

            //try
            //{
                
            //    if (oConn.State != ConnectionState.Open)
            //    {
            //        oConn.Open();                    
            //    }             

            //   // oComm.ExecuteNonQuery();

            //}
            //catch (Exception ex)
            //{                
            //    LogSQLError(ex.ToString(), sHL7);
            //    Console.WriteLine("SQL Error Occurred and was logged");

            //    //send NAK
            //    bNAK = true;

            //}
            //finally
            //{
            //    // need to do this on dispose of class obj (so db conn is not called billions of times)!
                
            //    //oConn.Close();
            //    //oConn.Dispose();
            //}

            
 



        }


        public void ProcessA01(string sHL7)
        {
            var parser = new NHapi.Base.Parser.PipeParser();
            var parsedMessage = parser.Parse(sHL7);
            var a01 = parsedMessage as ADT_A01;

            string sPtName = "";

            //Terser terser;
            
            Terser terser = new Terser(parsedMessage);

            
            sPtName = terser.Get("/PID-5");
            string sFName = terser.Get("/PID-5-2");
            string sLName = terser.Get("/PID-5-1");

            //attempt parsing of a field that does not exist in transaction
            string sPatientID = terser.Get("/PID-1");

            Console.WriteLine("Patient First Name = {0}", sFName);
            Console.WriteLine("Patient Last Name = {0}", sLName);


            
            //var patientMR = a01.PID.PatientID.IDNumber.Value;
            //var patientAcctNo= a01.PID.PatientAccountNumber.IDNumber.Value;
            //var patientName = a01.PID.GetPatientName(0);
            //string sLName = patientName.FamilyName.Surname.Value;
            //string sFName = patientName.GivenName.Value;

            //Console.WriteLine("Patient FName = {0}", sFName);



        }

        public static String GetHL7Event(String message)
        {
            var parser = new NHapi.Base.Parser.PipeParser();
            var parsedMessage = parser.Parse(message);

            //Terse it!
            Terser terser = new Terser(parsedMessage);

            //Grab RequiredFields

            string sEventType = terser.Get("/MSH-9-1");
            string sMessageType = terser.Get("/MSH-9-2");


            //Get the message type and trigger event
            var msgType = parsedMessage.GetStructureName();

            //Console.WriteLine("msg type = {0}", sMessageType); // = ADT_EVENT

            //Get the message in raw, pipe-delimited format
            var pipeDelimitedMessage = parser.Encode(parsedMessage);


            //Console.WriteLine("msg = {0}", pipeDelimitedMessage);            

            return sMessageType; //msgType; //pipeDelimitedMessage;


        }


        public static String ParseMessage(String message)
        {
            var parser = new NHapi.Base.Parser.PipeParser();
            var parsedMessage = parser.Parse(message);

            //Get the message type and trigger event
            var msgType = parsedMessage.GetStructureName();

            //Console.WriteLine("msg type = {0}", msgType); // = ADT_EVENT

            //Get the message in raw, pipe-delimited format
            var pipeDelimitedMessage = parser.Encode(parsedMessage);


            //Console.WriteLine("msg = {0}", pipeDelimitedMessage);            
            
            return pipeDelimitedMessage;


        }



        //private void ProcessPatient(HL7Message message)
        //{
        //    // Is Patient already in DB?
        //    // create patient if not
        //    // (only process required HL7 events) = extract these from DB




        //}


        //private void ProcessVisit(HL7Message message)
        //{


        //}




        void IDisposable.Dispose()
        {
            oConn.Close();
            oConn.Dispose();             
        }



    }
}
