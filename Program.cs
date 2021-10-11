using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.SqlServer.Server;

namespace UDPBroadcastSender
{
     class Program
    {
        static void Main(string[] args)
        {
            int pPort = 00000;
            Console.WriteLine("input port #:");
            string t = Console.ReadLine();
            try
            {
                pPort = Convert.ToInt32(t);
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed due to : "+e.ToString());
            }
            Console.WriteLine("Sending on Port: " + pPort);
            //using socket constructor takes 3 parameters~
            //address family interNetwork means we are using IPv4
            //SocketType Dgram means we are using dataGram media
            //ProtocolType Udp means we are using Udp protocols
            Socket sockBroadCaster = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //set flag enable broadcast to true so we can broadcast
            sockBroadCaster.EnableBroadcast = true;
                //IP endpoint this cannot take a string variable, this can be overcome using the Parse command IN THE CORRECT ADDRESS FAMILY YOU ARE USING!!
            //IP Endpoint is is where you are sending the data to!
            //IPEndpoint take 2 params
            //first is the IP address
            //second is the Port
            //ensure you are matching the same family as the Socket declared earlier on line 19
            //IPAddress.Parse parses the string value based on IP Address format, and pPort is the port used
            IPEndPoint broadcastEp = new IPEndPoint(IPAddress.Parse("255.255.255.255"), pPort);
            //define an array of bytes to contain data that will be broadcasted
            //{0x0D, 0x0A}stands for /r/n
            byte[] broadCastBuffer = new byte[] {0x0D, 0x0A};

            //this Identifies the Ip Sender to validate they are acceptable
            IPEndPoint ipEpSender = new IPEndPoint(IPAddress.Any, 0);
            //this establishes who the Sender is 
            EndPoint eSender = (EndPoint)ipEpSender;
            try
            {
                //binds the connection for the sender ~the any means this program is accessible to any connection on the machine
                sockBroadCaster.Bind(new IPEndPoint(IPAddress.Any, 0));

                //put into while loop to keep system running
                while (true)
                {
                    //this is to put data on the wire and test it
                    Console.WriteLine("Input Test Data, Type <Exit> to exit");
                    string uI = Console.ReadLine();
                    //converts the info to ASCII Coding for readability
                    broadCastBuffer = Encoding.ASCII.GetBytes(uI);
                    //this fires the data out the point
                    sockBroadCaster.SendTo(broadCastBuffer, broadcastEp);
                    if (uI.Equals("<ECHO>"))
                    {
                        int nCRcv = 0;
                        string dataRcvd = string.Empty;
                        //this shows the received from info and Info Sent~ takes 2 parameters
                        //the byte array buffer 
                        //and the EndPointSender
                        nCRcv = sockBroadCaster.ReceiveFrom(broadCastBuffer, ref eSender);
                        //this takes in the data and converts it to readable info in ACSII format
                        //this takes 3 parameters
                        //bytes is # of Bytes from the buffer array //!receiveBuffer!//
                        //Index is the index starting point for the conversion (i.e.0) //!0!//
                        //count is the count of bytes in the broadcast the number of bytes to convert in the broadcast //!nCountReceived!//
                        dataRcvd = Encoding.ASCII.GetString(broadCastBuffer, 0, nCRcv);
                        //ouput and validate if the data has been received
                        Console.WriteLine("# Bytes Rcv: " + nCRcv + Environment.NewLine
                                          + "Info: " + dataRcvd + Environment.NewLine + "from: "
                                          + eSender.ToString());

                    }





                }
                //the shutdown method is going to disable all operations on the socket object 
                sockBroadCaster.Shutdown(SocketShutdown.Both);
                //call the close method which will free the socket up
                sockBroadCaster.Close();

            }
            //if you get this far you've broken something... next step is to figure out what
            catch (Exception ex)
            {
                //writes the error that is caused to the console
                Console.WriteLine(ex.ToString());
            }


        }
    }
}
