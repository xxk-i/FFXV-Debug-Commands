using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXVHook
{

    public class InjectionEntryPoint : EasyHook.IEntryPoint
    {
        UInt64 OnSelectPlayerChangeThis;

        ServerInterface _server = null;
        Queue<String> _messageQueue = new Queue<string>();

        int clientPID = EasyHook.RemoteHooking.GetCurrentProcessId();

        public InjectionEntryPoint(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            _server = EasyHook.RemoteHooking.IpcConnectClient<ServerInterface>(channelName);
            _server.Ping();
        }

        #region OnSelectPlayerChangeMenu_Hook

        //OnSelectPlayerChangeMenu_Hook captures the *this pointer to save for use later
        //then calls the original function
        void OnSelectPlayerChangeMenu_Hook(UInt64 ptrPlayerChangeManager, int index)
        {
            OnSelectPlayerChangeThis = ptrPlayerChangeManager;
            FunctionImports.OnSelectPlayerChangeMenuFunc(ptrPlayerChangeManager, index);
        }
        #endregion

        public void Run(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            _server.IsInstalled(clientPID);

            //Install hooks
            var OnSelectPlayerChangeMenuHook = EasyHook.LocalHook.Create(FunctionImports.OnSelectPlayerChangeMenuAddr, new FunctionImports.OnSelectPlayerChangeMenu(OnSelectPlayerChangeMenu_Hook), null);

            //Activate hooks
            OnSelectPlayerChangeMenuHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

            //Collect OnSelectPlayerChangeThis for calling OnSelectPlayerChangeMenu
            OnSelectPlayerChangeThis = FunctionImports.GetPlayerChangeManagerFunc();

            /*
             * 
             * TODO TODO TODO TODO
             * Enable option in debug struct to allow swtiching to bros at any point
             * 
             */

            _server.ReportMessage("Collected pointer: " + OnSelectPlayerChangeThis.ToString("X"));

            //Call it and switch to the guest
            FunctionImports.OnSelectPlayerChangeMenuFunc(OnSelectPlayerChangeThis, 3);

            //TODO log this in GUI console tab
            //_server.ReportMessage("Created correctly? Check: " + (FunctionImports.modBase + 0x8BEF10).ToString("X"));

            try
            {
                while (true)
                {
                    if (OnSelectPlayerChangeThis != 0)
                    {
                        _server.ReportMessage("Captured ptrActorManager: " + OnSelectPlayerChangeThis.ToString("X"));
                    }
                    System.Threading.Thread.Sleep(500);

                    string[] queued = null;

                    lock (_messageQueue)
                    {
                        queued = _messageQueue.ToArray();
                        _messageQueue.Clear();
                    }

                    // Send newly monitored file accesses to FileMonitor
                    if (queued != null && queued.Length > 0)
                    {
                        _server.ReportMessages(queued);
                    }
                    else
                    {
                        _server.Ping();
                    }
                }
            }
            catch
            {

            }

            //Remove hooks
            OnSelectPlayerChangeMenuHook.Dispose();

            EasyHook.LocalHook.Release();
            return;
        }
        
        public static void WhatsUp()
        {
            Console.WriteLine("Hi");
        }
    }
}

