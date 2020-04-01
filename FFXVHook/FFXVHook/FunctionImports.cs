using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFXVHook
{
    class FunctionImports
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        public static IntPtr modBase = GetModuleHandle(null);

        #region Game Function Delegates

        //Refers to in battle character switching menu, can also be used to switch to unlisted index 4 (guest)
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void OnSelectPlayerChangeMenu(UInt64 ptrPlayerChangeManager, int index);

        //Returns whether player changing is allowed
        //Checked in a loop, if it returns false the game switches player back to noctis
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate bool PlayerChangeManagerIsEnabled(UInt64 ptrPlayerChangeManager);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void PlayerChangeManagerUpdate(UInt64 ptrPlayerChangeManager, UInt64 deltaTime);

        //SetUserControlActor does not enable attacking, do not bother using with bros
        //Game calls from debug menu with following bools set:
        //  bChangePadType = true
        //  bDebugPad = false;
        //  bSave = true;
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void SetUserControlActor(UInt64 ptrActorManager, UInt64 actor, bool bChangePadType, bool bDebugPad, bool bSave);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void SetUserControlPlayer(UInt64 ptrActorManager, UInt64 actor);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetPartyActor(UInt64 ptrActorManager, int party_index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetActorFromCharacterEntryID(UInt64 ptrActorManger, int manager_type, uint character_entry_id);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetActorFromIndex(UInt64 ptrActorTypeManager, uint index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetActorTypeManager();

        #endregion Game Function Delegates

        #region Engine Function Delegates

        //SQEX::Luminous::Core::Singleton<Black::System::PlayerChange::PlayerChangeManager>::Get()
        //Returns PlayerChangeManager instance (pointer) to use for its class member functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetPlayerChangeManager();

        //Black::ActorManager::ActorManager *__fastcall Black::ActorManager::ActorManager::GetInstance()
        //Returns ActorManager instance (pointer) to use for its class member functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetActorManagerInstance();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetJobCommandManagerInstance();

        #endregion Engine Function Delegates


        #region Debug Offsets
        //For debug, seems to be IDA file offset + 0xC00
        public static IntPtr dbOnSelectPlayerChangeMenuAddr = (modBase + 0x8BEF10);
        public IntPtr dbPlayerChangeManagerIsEnabledAddr = (modBase + 0x8BE4C0);
        public static IntPtr dbSetUserControlActorAddr = (modBase + 0x5CD1860);

        public static IntPtr dbGetPlayerChangeManagerAddr = (modBase + 0xC8CA0);
        public static IntPtr dbGetActorManagerInstanceAddr = (modBase + 0x480080);
        public static IntPtr dbGetJobCommandManagerAddr = (modBase + 0x22C1F0);
        #endregion Debug Offsets

        #region Release Offsets
        //Also seems to be IDA file offset + 0xC00
        //public static IntPtr OnSelectPlayerChangeMenuAddr = (modBase + 0x898F50);  //IDA file offset 0x898350
        public static IntPtr OnSelectPlayerChangeMenuAddr = (modBase + 0x898F70);
        public IntPtr PlayerChangeManagerIsEnabledAddr = (modBase + 0x1082CD0);
        public static IntPtr PlayerChangeManagerUpdateAddr = (modBase + 0x594D60);
        //public static IntPtr SetUserControlActorAddr = (modBase + 0x4F24060);   //IDA file offset 0x4F23460
        public static IntPtr SetUserControlActorAddr = (modBase + 0x5CA4660);
        public static IntPtr SetUserControlPlayerAddr = (modBase + 0x5CA4C90);

        public static IntPtr GetPartyActorAddr = (modBase + 0x5CAD4D0);
        public static IntPtr GetPlayerChangeManagerAddr = (modBase + 0x5C9D760);
        public static IntPtr GetActorManagerInstanceAddr = (modBase + 0x6A98AE0);
        public static IntPtr GetActorFromCharacterEntryIDAddr = (modBase + 0x7893350);
        public static IntPtr GetActorFromIndexAddr = (modBase + 0x1225170);
        #endregion Release Offsets

        #region Functions
        public OnSelectPlayerChangeMenu OnSelectPlayerChangeMenuFunc;
        public PlayerChangeManagerIsEnabled PlayerChangeManagerIsEnabledFunc;
        public PlayerChangeManagerUpdate PlayerChangeManagerUpdateFunc;
        public SetUserControlActor SetUserControlActorFunc;
        public SetUserControlPlayer SetUserControlPlayerFunc;

        public GetActorFromCharacterEntryID GetActorFromCharacterEntryIDFunc;
        public GetPartyActor GetPartyActorFunc;
        public GetActorManagerInstance GetActorManagerInstanceFunc;
        public GetPlayerChangeManager GetPlayerChangeManagerFunc;
        public GetJobCommandManagerInstance GetJobCommandManagerFunc;
        public GetActorFromIndex GetActorFromIndexFunc;
        #endregion

        public FunctionImports(bool debug)
        {
            if(debug)
            {
                OnSelectPlayerChangeMenuFunc = Marshal.GetDelegateForFunctionPointer<OnSelectPlayerChangeMenu>(dbOnSelectPlayerChangeMenuAddr);
                PlayerChangeManagerIsEnabledFunc = Marshal.GetDelegateForFunctionPointer<PlayerChangeManagerIsEnabled>(dbPlayerChangeManagerIsEnabledAddr);
                SetUserControlActorFunc = Marshal.GetDelegateForFunctionPointer<SetUserControlActor>(dbSetUserControlActorAddr);

                GetPlayerChangeManagerFunc = Marshal.GetDelegateForFunctionPointer<GetPlayerChangeManager>(dbGetPlayerChangeManagerAddr);
                GetActorManagerInstanceFunc = Marshal.GetDelegateForFunctionPointer<GetActorManagerInstance>(dbGetActorManagerInstanceAddr);
                GetJobCommandManagerFunc = Marshal.GetDelegateForFunctionPointer<GetJobCommandManagerInstance>(dbGetJobCommandManagerAddr);
            }
            
            else
            {
                OnSelectPlayerChangeMenuFunc = Marshal.GetDelegateForFunctionPointer<OnSelectPlayerChangeMenu>(OnSelectPlayerChangeMenuAddr);
                PlayerChangeManagerIsEnabledFunc = Marshal.GetDelegateForFunctionPointer<PlayerChangeManagerIsEnabled>(PlayerChangeManagerIsEnabledAddr);
                PlayerChangeManagerUpdateFunc = Marshal.GetDelegateForFunctionPointer<PlayerChangeManagerUpdate>(PlayerChangeManagerUpdateAddr);
                SetUserControlActorFunc = Marshal.GetDelegateForFunctionPointer<SetUserControlActor>(SetUserControlActorAddr);
                SetUserControlPlayerFunc = Marshal.GetDelegateForFunctionPointer<SetUserControlPlayer>(SetUserControlPlayerAddr);

                GetActorFromIndexFunc = Marshal.GetDelegateForFunctionPointer<GetActorFromIndex>(GetActorFromIndexAddr);
                GetActorFromCharacterEntryIDFunc = Marshal.GetDelegateForFunctionPointer<GetActorFromCharacterEntryID>(GetActorFromCharacterEntryIDAddr);
                GetPartyActorFunc = Marshal.GetDelegateForFunctionPointer<GetPartyActor>(GetPartyActorAddr);
                GetPlayerChangeManagerFunc = Marshal.GetDelegateForFunctionPointer<GetPlayerChangeManager>(GetPlayerChangeManagerAddr);
                GetActorManagerInstanceFunc = Marshal.GetDelegateForFunctionPointer<GetActorManagerInstance>(GetActorManagerInstanceAddr);
            }
        }
    }
}