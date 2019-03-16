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
            FunctionImports.OnSelectPlayerChangeMenuDelegate test = new FunctionImports.OnSelectPlayerChangeMenuDelegate(OnSelectPlayerChangeMenu_Hook);
            //_server.ReportMessage((modBase + 0xD1D80).ToString());

            var OnSelectPlayerChangeMenuHook = EasyHook.LocalHook.Create(FunctionImports.OnSelectPlayerChangeMenuAddr, test, null);
            OnSelectPlayerChangeMenuHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

            _server.ReportMessage(EasyHook.HookRuntimeInfo.IsHandlerContext.ToString());

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
                    //_server.ReportMessage("maybe");

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
        
    }
}

