using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXVHook
{
    public class ServerInterface : MarshalByRefObject
    {
        string otherChannel;
        DLLServer dllServer;
        bool debug;

        //Called when DLL is injected
        public void IsInstalled(int clientPID, string otherChannel) 
        {
            //this.Injection = Injection;
            this.otherChannel = otherChannel;
            dllServer = EasyHook.RemoteHooking.IpcConnectClient<DLLServer>(otherChannel);
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

        //Reports message through messagebox
        public void ReportUrgentMessage(string message)
        {
            MessageBox.Show(message);
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

        public void SwitchCharacter(int index)
        {
            Console.WriteLine("Received character switch");
            gdllServer.SwitchCharacter(index);
        }
        
        public void SwitchBattleCharacter(int index)
        {
            Console.WriteLine("Recieved battle character swtich");
            dllServer.SwitchBattleCharacter(index);
        }

        public void SwitchCharacterCustom(UInt64 customHandle)
        {
            Console.WriteLine("Received custom character switch!");
            dllServer.SwitchCharacterCustom(customHandle);
        }

        public void Disconnect()
        {
            Console.WriteLine("Receieved disconnect message");
            dllServer.OtherDisconnect();
        }

        public void SetDebug(bool option)
        {
            debug = option;
            Console.WriteLine("DEBUG set to " + option);
        }

        public bool GetDebug()
        {
            return false;
        }
    }
}
