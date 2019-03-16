using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXVHook
{
    public class ServerInterface : MarshalByRefObject
    {
        //Called when DLL is injected
        public void IsInstalled(int clientPID)
        {
            Console.WriteLine("Successfully injected into process {0}.\r\n", clientPID);
        }

        //Output messages to console
        public void ReportMessages(string[] messages)
        {
            foreach (string message in messages)
            {
                Console.WriteLine(messages);
            }
        }

        public void ReportMessage(string message)
        {
            Console.WriteLine(message);
        }

        //report exceptions
        public void ReportException(Exception e)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + e.ToString());
        }

        //Called to confirm the IPC channel is still open/host application has not closed
        public void Ping()
        {

        }

        public static void RunTheThing()
        {
            InjectionEntryPoint.WhatsUp();
        }
    }
}
