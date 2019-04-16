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

        //SetUserControlActor does not enable attacking, do not bother using with bros
        //Game calls from debug menu with following bools set:
        //  bChangePadType = true
        //  bDebugPad = false;
        //  bSave = true;
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void SetUserControlActor(UInt64 ptrActorManager, UInt64 actor, bool bChangePadType, bool bDebugPad, bool bSave);

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
        public static IntPtr OnSelectPlayerChangeMenuAddr = (modBase + 0x898F50);  //IDA file offset 0x898350
        public IntPtr PlayerChangeManagerIsEnabledAddr = (modBase + 0x1);
        public static IntPtr SetUserControlActorAddr = (modBase + 0x4F23460);   //IDA file offset 0x4F23460

        public static IntPtr GetPlayerChangeManagerAddr = (modBase + 0xB7C60);
        public static IntPtr GetActorManagerInstanceAddr = (modBase + 0x1);
        #endregion Release Offsets

        #region Functions
        public OnSelectPlayerChangeMenu OnSelectPlayerChangeMenuFunc;
        public PlayerChangeManagerIsEnabled PlayerChangeManagerIsEnabledFunc;
        public SetUserControlActor SetUserControlActorFunc;

        public GetActorManagerInstance GetActorManagerInstanceFunc;
        public GetPlayerChangeManager GetPlayerChangeManagerFunc;
        public GetJobCommandManagerInstance GetJobCommandManagerFunc;
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
                SetUserControlActorFunc = Marshal.GetDelegateForFunctionPointer<SetUserControlActor>(SetUserControlActorAddr);

                GetPlayerChangeManagerFunc = Marshal.GetDelegateForFunctionPointer<GetPlayerChangeManager>(GetPlayerChangeManagerAddr);
                GetActorManagerInstanceFunc = Marshal.GetDelegateForFunctionPointer<GetActorManagerInstance>(GetActorManagerInstanceAddr);
            }
        }
    }
}