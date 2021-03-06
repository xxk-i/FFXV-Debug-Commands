﻿using System;
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

        ServerInterface _server = null;
        Queue<String> _messageQueue = new Queue<string>();
        FunctionImports functions;

        int clientPID = EasyHook.RemoteHooking.GetCurrentProcessId();
        string channelName2 = null;

        UInt64 ptrManager;

        //Hooks
        EasyHook.LocalHook OnSelectPlayerChangeMenuHook;
        EasyHook.LocalHook PlayerChangeManagerIsEnabledHook;
        EasyHook.LocalHook PlayerChangeManagerUpdateHook;
        EasyHook.LocalHook GetActorFromCharacterEntryIDHook;

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
                EasyHook.RemoteHooking.IpcCreateServer<DLLServer>(ref channelName2, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
            }
            catch (Exception e)
            {
                _server.ReportException(e);
            }
        }

        ~InjectionEntryPoint()
        {
            _server.ReportUrgentMessage("Injection removed by game, re-inject!");
        }

        #region OnSelectPlayerChangeMenu_Hook
        //Currently unnecessary, see GetPlayerChangeManager
        //OnSelectPlayerChangeMenu_Hook captures the *this pointer to save for use later
        //then calls the original function
        void OnSelectPlayerChangeMenu_Hook(UInt64 ptrPlayerChangeManager, int index)
        {
            functions.OnSelectPlayerChangeMenuFunc(ptrPlayerChangeManager, index);
        }
        #endregion

        bool PlayerChangeManagerIsEnabled_Hook(UInt64 ptrPlayerChangeManager)
        {
            return true;
        }
        
        void PlayerChangeManagerUpdate_Hook(UInt64 ptrPlayerChangeManager, UInt64 deltaTime)
        {
            ptrManager = ptrPlayerChangeManager;
            functions.PlayerChangeManagerUpdateFunc(ptrPlayerChangeManager, deltaTime);
        }

        public void Run(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            _server.ReportMessage("InjectionEntryPoint Run:");
            _server.IsInstalled(clientPID, channelName2);

            functions = new FunctionImports(_server.GetDebug());

            //Install hooks
            PlayerChangeManagerIsEnabledHook = EasyHook.LocalHook.Create(functions.dbPlayerChangeManagerIsEnabledAddr, new FunctionImports.PlayerChangeManagerIsEnabled(PlayerChangeManagerIsEnabled_Hook), null);
            OnSelectPlayerChangeMenuHook = EasyHook.LocalHook.Create(FunctionImports.OnSelectPlayerChangeMenuAddr, new FunctionImports.OnSelectPlayerChangeMenu(OnSelectPlayerChangeMenu_Hook), null);
            PlayerChangeManagerUpdateHook = EasyHook.LocalHook.Create(FunctionImports.PlayerChangeManagerUpdateAddr, new FunctionImports.PlayerChangeManagerUpdate(PlayerChangeManagerUpdate_Hook), null);

            //Activate hooks
            PlayerChangeManagerIsEnabledHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            OnSelectPlayerChangeMenuHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            PlayerChangeManagerUpdateHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

            //0x5137Ab0
            //0x1aa58e780
            //functions.SetUserControlActorFunc(0x5137AB0, 0x1AA59E3F0, true, false, true);

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
            //Collect OnSelectPlayerChangeThis for calling OnSelectPlayerChangeMenu
            //UInt64 onSelectPlayerChangeThis = functions.GetPlayerChangeManagerFunc();

            //UInt64 isAllOpenForDegugMode = (onSelectPlayerChangeThis + 0xC8);
            //UInt64 isAllowNonBattle = (onSelectPlayerChangeThis + 0xCA);

            /*
            This is debug version only, offsets are different or bools don't exist in release.
            For same effect use the PlayerChangeManagerIsEnabled hook to always return true.
            unsafe
            {
                //Both of these bools must be set or else game will force character to switch back
                _server.ReportMessage("isAllOpenForDebugMode: " + *(bool*)isAllOpenForDegugMode);
                *((bool*)isAllOpenForDegugMode) = true;
                _server.ReportMessage("isAllOpenForDebugMode changed to: " + *(bool*)isAllOpenForDegugMode + " at " + isAllOpenForDegugMode.ToString());

                _server.ReportMessage("isAllowNonBattle: " + *(bool*)isAllowNonBattle);
                *((bool*)isAllowNonBattle) = true;
                _server.ReportMessage("isAllowNonBattle changed to: " + *(bool*)isAllowNonBattle + " at " + isAllowNonBattle.ToString());
            }
            */

            /*
            _server.ReportMessage(ptrManager.ToString());
            _server.ReportMessage("Changing character to index " + index);
            functions.OnSelectPlayerChangeMenuFunc(functions.GetPlayerChangeManagerFunc(), index);
            _server.ReportMessage("after OnSelectPlayerChangeMenuFunc");
            */

            /*
            UInt64 actorManager = functions.GetActorManagerInstanceFunc();
            UInt64 partyActorPtr = functions.GetPartyActorFunc(actorManager, index);
            functions.SetUserControlActorFunc(actorManager, partyActorPtr, true, false, true);
            */

            UInt64 actorManager = functions.GetActorManagerInstanceFunc();
            UInt64 actorTypeManager = functions.GetActorTypeManager();
            UInt64 actorToSwitchTo = functions.GetActorFromIndexFunc(actorTypeManager, 5);
            
          
        }

        public void SwitchBattleCharacter(int index)
        {
            _server.ReportMessage("Switching to battle actor: " + index);
            functions.OnSelectPlayerChangeMenuFunc(functions.GetPlayerChangeManagerFunc(), index);
        }
        
        public void SwitchCharacterCustom(UInt64 customHandle)
        {
            UInt64 actorManagerThis = functions.GetActorManagerInstanceFunc();
            functions.SetUserControlActorFunc(actorManagerThis, customHandle, true, false, true); 
        }

        public void Disconnect()
        {
            sInstance = null;
            _server.ReportMessage("Uninstalling and releasing");

            // Remove hooks
            //OnSelectPlayerChangeMenuHook.Dispose();
            EasyHook.LocalHook.Release();
        }
    }

    public class DLLServer : MarshalByRefObject
    {
        public void OtherDisconnect()
        {
            InjectionEntryPoint.sInstance?.Disconnect();
        }

        public void SwitchCharacter(int index)
        {
            InjectionEntryPoint.sInstance?.SwitchCharacter(index);
        }

        public void SwitchBattleCharacter(int index)
        {
            InjectionEntryPoint.sInstance?.SwitchBattleCharacter(index);
        }

        public void SwitchCharacterCustom(UInt64 customHandle)
        {
            InjectionEntryPoint.sInstance?.SwitchCharacterCustom(customHandle);
        }
    }
}

