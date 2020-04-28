using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Data;


namespace HL7ListenerDatabase
{
    class HL7TCPListener : IDisposable
    {
        const int TCP_TIMEOUT = 300000; // timeout value for receiving TCP data in millseconds
        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private int listernerPort;
        private bool sendACK = true;
        private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        private bool runThread = true;
        // ---- Database Objects
        SqlConnection oConn;
        SqlCommand oComm;
        private bool sendNAK = false;
        private Logger logger;
        private string messageType;


        //private HL7Processor hl7Processor;
        public HL7TCPListener(int port, string databaseConnectionString, string StoredProcedureName, string LogFileLocation, Int32 LogHistroyLimit, Int32 ErrorHistroyLimit)
        {
            this.listernerPort = port;
            oConn = new SqlConnection(databaseConnectionString);
            oComm = new SqlCommand();
            oComm.Connection = oConn;
            oComm.CommandType = CommandType.StoredProcedure;
            oComm.CommandText = StoredProcedureName;
            oComm.Parameters.Add("@HL7Data", SqlDbType.NVarChar, -1);
            oComm.Parameters.Add("@MessageType", SqlDbType.NVarChar, 3);
            logger = new Logger(LogFileLocation, LogHistroyLimit, ErrorHistroyLimit);
            //hl7Processor = new HL7Processor();
        }

        public bool Start()
        {
            // start a new thread to listen for new TCP conmections
            this.tcpListener = new TcpListener(IPAddress.Any, this.listernerPort);
            this.tcpListenerThread = new Thread(new ThreadStart(StartListener));
            this.tcpListenerThread.Start();
            logger.AddLog("Starting HL7 listener on port " + this.listernerPort);
            //this.LogInformation("# Starting HL7 listener on port " + this.listernerPort);
            return true;
        }

        public void RequestStop()
        {
            this.runThread = false;
            sendNAK = true;
        }

        /// <summary>
        /// Start listening for new connections
        /// </summary>
        private void StartListener()
        {
            try
            {
                this.tcpListener.Start();
                // run the thread unless a request to stop is received
                while (this.runThread)
                {
                    // waits for a client connection to the listener
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    logger.AddLog("New client connection accepted from " + client.Client.RemoteEndPoint);
                    
                    // create a new thread. This will handle communication with a client once connected
                    Thread clientThread = new Thread(new ParameterizedThreadStart(ReceiveData));
                    clientThread.Start(client);
                }
            }
            catch (Exception e)
            {
                logger.AddError("An error occurred while attempting to start the listener on port " + this.listernerPort);
                logger.AddError(e.Message);
                logger.AddError("HL7Listener exiting.");
            }
        }

        /// <summary>
        /// Receive data from a client connection, look for MLLP HL7 message.
        /// </summary>
        /// <param name="client"></param>
        private void ReceiveData(object client)
        {

            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] messageBuffer = new byte[4096];
            int bytesRead;
            String messageData = "";

            String sHL7 = "";

            int messageCount = 0;

            //Console.WriteLine("looking for HL7...");

            while (true)
            {
                bytesRead = 0;
                try
                {
                    // Wait until a client application submits a message
                    bytesRead = clientStream.Read(messageBuffer, 0, 4096);
                }
                catch (Exception e)
                {
                    // A network error has occurred
                    logger.AddLog("Connection from " + tcpClient.Client.RemoteEndPoint + " has ended. " + e.Message);
                    break;
                }
                if (bytesRead == 0)
                {
                    // The client has disconected
                    logger.AddLog("The client " + tcpClient.Client.RemoteEndPoint + " has disconnected");
                    break;
                }
                // Message buffer received successfully
                messageData += Encoding.UTF8.GetString(messageBuffer, 0, bytesRead);
                // Find a VT character, this is the beginning of the MLLP frame

                logger.AddLog("read HL7 first line into buffer...");

                int start = messageData.IndexOf((char)0x0B);
                if (start >= 0)
                {

                    logger.AddLog("found index");

                    // Search for the end of the MLLP frame (a FS character)
                    int end = messageData.IndexOf((char)0x1C);
                    if (end > start)
                    {

                        logger.AddLog("found end of message");

                        messageCount++;
                        try
                        {

                            // create a HL7message object from the message recieved. Use this to access elements needed to populate the ACK message and file name of the archived message
                            HL7Message message = new HL7Message(messageData.Substring(start + 1, end - (start + 1)));

                            // get the HL7 message as a text string that can be processed with nHAPI (Mike B)
                            sHL7 = messageData.Substring(start + 1, end - (start + 1));

                            messageType = message.GetHL7Item("MSH-9.1")[0];
                            SaveMessageToDatabase(sHL7);
                           
                            if (sendNAK == false)
                            { //send ACK

                                messageData = ""; // reset the message data string for the next message
                                string messageTrigger = message.GetMessageTrigger();
                                string messageControlID = message.GetHL7Item("MSH-10")[0];
                                string acceptAckType = "AL"; //message.GetHL7Item("MSH-15")[0];
                                string dateStamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                                // send ACK message is MSH-15 is set to AL and ACKs not disbaled by -NOACK command line switch
                                if ((this.sendACK) && (acceptAckType.ToUpper() == "AL"))
                                {
                                    logger.AddLog("Sending ACK (Message Control ID: " + messageControlID + ")");
                                    // generate ACK Message and send in response to the message received
                                    string response = GenerateACK(message.ToString());  // TO DO: send ACKS if set in message header, or specified on command line

                                    logger.AddLog("ACK = " + response);

                                    byte[] encodedResponse = Encoding.UTF8.GetBytes(response);
                                    // Send response
                                    try
                                    {

                                        //MB this is where you could also send a NAK

                                        clientStream.Write(encodedResponse, 0, encodedResponse.Length);
                                        clientStream.Flush();
                                    }
                                    catch (Exception e)
                                    {
                                        // A network error has occurred
                                        logger.AddError("An error has occurred while sending an ACK to the client " + tcpClient.Client.RemoteEndPoint);
                                        logger.AddError(e.Message);
                                        break;
                                    }
                                }


                            }
                            else //send NAK
                            {

                                messageData = ""; // reset the message data string for the next message
                                string messageTrigger = message.GetMessageTrigger();
                                string messageControlID = message.GetHL7Item("MSH-10")[0];
                                string acceptAckType = "AL"; //message.GetHL7Item("MSH-15")[0];
                                string dateStamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();

                                // send ACK message is MSH-15 is set to AL and ACKs not disbaled by -NOACK command line switch
                                if ((this.sendACK) && (acceptAckType.ToUpper() == "AL"))
                                {
                                    //LogInformation("Sending NAK (Message Control ID: " + messageControlID + ")");
                                    // generate ACK Message and send in response to the message received
                                    string response = GenerateNAK(message.ToString());  // TO DO: send ACKS if set in message header, or specified on command line

                                    logger.AddLog("ACK = " + response);

                                    byte[] encodedResponse = Encoding.UTF8.GetBytes(response);
                                    // Send response
                                    try
                                    {

                                        //MB this is where you could also send a NAK

                                        clientStream.Write(encodedResponse, 0, encodedResponse.Length);
                                        clientStream.Flush();
                                    }
                                    catch (Exception e)
                                    {
                                        // A network error has occurred
                                        logger.AddError("An error has occurred while sending an NAK to the client " + tcpClient.Client.RemoteEndPoint);
                                        logger.AddError(e.Message);
                                        break;
                                    }
                                }


                                //MB To do: Disconnect Listener after NAK
                                this.RequestStop();



                            }

                        }

                        // if false NAK

                        catch (Exception e)
                        {
                            messageData = ""; // reset the message data string for the next message
                            logger.AddError("An exception occurred while parsing the HL7 message");
                            logger.AddError(e.Message);
                            break;
                        }
                    }
                }
            }
            //LogInformation("Total messages received:" + messageCount);
            clientStream.Close();
            clientStream.Dispose();
            tcpClient.Close();
        }

        /// <summary>
        /// Generate a string containing the ACK message in response to the original message. Supply a string containing the original message (or at least the MSH segment).
        /// </summary>
        /// <returns></returns>
        string GenerateACK(string originalMessage)
        {
            // create a HL7Message object using the original message as the source to obtain details to reflect back in the ACK message
            HL7Message tmpMsg = new HL7Message(originalMessage);
            string trigger = tmpMsg.GetHL7Item("MSH-9.2")[0];
            string originatingApp = tmpMsg.GetHL7Item("MSH-3")[0];
            string originatingSite = tmpMsg.GetHL7Item("MSH-4")[0];
            string messageID = tmpMsg.GetHL7Item("MSH-10")[0];
            string processingID = "T"; //tmpMsg.GetHL7Item("MSH-11")[0];
            string hl7Version = "2.3"; //tmpMsg.GetHL7Item("MSH-12")[0];
            string ackTimestamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();

            StringBuilder ACKString = new StringBuilder();
            ACKString.Append((char)0x0B);
            ACKString.Append("MSH|^~\\&|HL7Listener|HL7Listener|" + originatingSite + "|" + originatingApp + "|" + ackTimestamp + "||ACK^" + trigger + "|" + messageID + "|" + processingID + "|" + hl7Version);
            ACKString.Append((char)0x0D);
            ACKString.Append("MSA|CA|" + messageID); // should be MSH|AA ???? MB
            ACKString.Append((char)0x1C);
            ACKString.Append((char)0x0D);
            return ACKString.ToString();
        }

        // MB added 11/28/16
        string GenerateNAK(string originalMessage)
        {
            // create a HL7Message object using the original message as the source to obtain details to reflect back in the ACK message
            HL7Message tmpMsg = new HL7Message(originalMessage);
            string trigger = tmpMsg.GetHL7Item("MSH-9.2")[0];
            string originatingApp = tmpMsg.GetHL7Item("MSH-3")[0];
            string originatingSite = tmpMsg.GetHL7Item("MSH-4")[0];
            string messageID = tmpMsg.GetHL7Item("MSH-10")[0];
            string processingID = "T"; //tmpMsg.GetHL7Item("MSH-11")[0];
            string hl7Version = "2.3"; //tmpMsg.GetHL7Item("MSH-12")[0];
            string ackTimestamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();

            StringBuilder NAKString = new StringBuilder();
            NAKString.Append((char)0x0B);
            NAKString.Append("MSH|^~\\&|HL7Listener|HL7Listener|" + originatingSite + "|" + originatingApp + "|" + ackTimestamp + "||ACK^" + trigger + "|" + messageID + "|" + processingID + "|" + hl7Version);
            NAKString.Append((char)0x0D);
            NAKString.Append("MSA|AR|" + messageID);
            NAKString.Append((char)0x1C);
            NAKString.Append((char)0x0D);
            return NAKString.ToString();

        }

        /// <summary>
        /// Set and get the values of the SendACK option. This can be used to overide sending of ACK messages. 
        /// </summary>
        public bool SendACK
        {
            get { return this.sendACK; }
            set { this.sendACK = value; }
        }
        private void SaveMessageToDatabase(String HL7Message)
        {
            //var parser = new NHapi.Base.Parser.PipeParser();
            //var parsedMessage = parser.Parse(HL7Message);

            //Terse it!
            //Terser terser = new Terser(parsedMessage);
           oComm.Parameters["@HL7Data"].Value = HL7Message;
           oComm.Parameters["@MessageType"].Value = messageType;
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
                //LogSQLError(ex.ToString(), HL7Message);
                logger.AddError(ex.ToString() + " SQL Error Occurred and was logged.");
                sendNAK = true;
            }
            finally
            { }

           
        }

        void IDisposable.Dispose()
        {
            oConn.Close();
            oConn.Dispose();
            oComm.Dispose();
            
        }
    }   
}
