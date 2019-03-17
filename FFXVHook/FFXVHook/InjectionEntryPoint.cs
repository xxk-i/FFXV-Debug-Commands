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
        public static InjectionEntryPoint sInstance = null;

        UInt64 OnSelectPlayerChangeThis;

        ServerInterface _server = null;
        Queue<String> _messageQueue = new Queue<string>();

        int clientPID = EasyHook.RemoteHooking.GetCurrentProcessId();
        string channelName2 = null;

        //Hooks
        EasyHook.LocalHook OnSelectPlayerChangeMenuHook;

        //Connects client and pings server to show injected DLL is responsive
        public InjectionEntryPoint(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            sInstance = this;
            _server = EasyHook.RemoteHooking.IpcConnectClient<ServerInterface>(channelName);
            _server.Ping();
            _server.ReportMessage("InjectionEntryPoint constructed");

            //Stand up a server in our process so we can know when to disconnect
            try
            {
                EasyHook.RemoteHooking.IpcCreateServer<OtherServer>(ref channelName2, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
            }
            catch (Exception e)
            {
                _server.ReportException(e);
            }
        }

        ~InjectionEntryPoint()
        {
            _server.ReportMessage("no dont kill me");
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
            _server.ReportMessage("InjectionEntryPoint Run:");
            _server.IsInstalled(clientPID, channelName2);

            //Install hooks
            OnSelectPlayerChangeMenuHook = EasyHook.LocalHook.Create(FunctionImports.OnSelectPlayerChangeMenuAddr, new FunctionImports.OnSelectPlayerChangeMenu(OnSelectPlayerChangeMenu_Hook), null);

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
            _server.ReportMessage("Collected OnSelectPlayerChange *this: " + OnSelectPlayerChangeThis.ToString("X"));

            //Set toggleAllOpenMode to allow character switching outside of battle
            UIntPtr isAllOpenForDegugMode = (UIntPtr)(OnSelectPlayerChangeThis + 0xC8);

            _server.ReportMessage("Setting isAllOpenForDebugMode at " + isAllOpenForDegugMode.ToString());

            unsafe
            {
                *((bool *)isAllOpenForDegugMode.ToPointer()) = true;
            }

            //Call it and switch to the guest
            FunctionImports.OnSelectPlayerChangeMenuFunc(OnSelectPlayerChangeThis, 3);
            //TODO log this in GUI console tab
            //_server.ReportMessage("Created correctly? Check: " + (FunctionImports.modBase + 0x8BEF10).ToString("X"));

            try
            {
                while (true)
                {
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

            return;
        }
    
        public void SwitchCharacter(int index)
        {
            _server.ReportMessage("Changing character to index " + index);
            FunctionImports.OnSelectPlayerChangeMenuFunc(OnSelectPlayerChangeThis, index);
        }
        
        public void Disconnect()
        {
            sInstance = null;
            _server.ReportMessage("Uninstalling and releasing");

            // Remove hooks
            OnSelectPlayerChangeMenuHook.Dispose();
            EasyHook.LocalHook.Release();
        }
    }

    public class OtherServer : MarshalByRefObject
    {
        public void OtherDisconnect()
        {
            InjectionEntryPoint.sInstance?.Disconnect();
        }

        public void SwitchCharacter(int index)
        {
            InjectionEntryPoint.sInstance?.SwitchCharacter(index);
        }

        public void DispatchCommand(string command)
        {
        }
    }
}

