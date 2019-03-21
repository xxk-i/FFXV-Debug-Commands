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

        //Function Delegates

        #region Game Functions

        //Refers to in battle character switching menu, can also be used to switch to unlisted index 4 (guest)
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void OnSelectPlayerChangeMenu(UInt64 ptrPlayerChangeManager, int index);

        //SetUserControlActor does not enable attacking, do not bother using with bros
        //Game calls from debug menu with following bools set:
        //  bChangePadType = true
        //  bDebugPad = false;
        //  bSave = true;
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void SetUserControlActor(UInt64 ptrActorManager, UInt64 actor, bool bChangePadType, bool bDebugPad, bool bSave);

        #endregion Game Functions


        #region Engine Functions

        //SQEX::Luminous::Core::Singleton<Black::System::PlayerChange::PlayerChangeManager>::Get()
        //Returns PlayerChangeManager instance (pointer) to use for its class member functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetPlayerChangeManager();

        //Black::ActorManager::ActorManager *__fastcall Black::ActorManager::ActorManager::GetInstance()
        //Returns ActorManager instance (pointer) to use for its class member functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetActorManagerInstance();

        #endregion Engine Functions


        //Debug Offsets
        //For debug, seems to be IDA file offset + 0xC00
        public static IntPtr dbOnSelectPlayerChangeMenuAddr = (modBase + 0x8BEF10);
        public static IntPtr dbSetUserControlActorAddr = (modBase + 0x5CD1860);

        public static IntPtr dbGetPlayerChangeManagerAddr = (modBase + 0xC8CA0);
        public static IntPtr dbGetActorManagerInstanceAddr = (modBase + 0x480080);  


        //Release Offsets
        //Also seems to be IDA file offset + 0xC00
        public static IntPtr OnSelectPlayerChangeMenuAddr = (modBase + 0x888E80);  //IDA file offset 0x11C6A60
        public static IntPtr GetPlayerChangeManagerAddr = (modBase + 0xB7C60);

        public static IntPtr SetUserControlActorAddr = (modBase + 0x4F23460);   //IDA file offset 0x4F23460
        public static IntPtr GetActorManagerInstanceAddr = (modBase + 0x1);



        //Original debug functions
        public static OnSelectPlayerChangeMenu dbOnSelectPlayerChangeMenuFunc = Marshal.GetDelegateForFunctionPointer<OnSelectPlayerChangeMenu>(dbOnSelectPlayerChangeMenuAddr);
        public static SetUserControlActor dbSetUserControlActorFunc = Marshal.GetDelegateForFunctionPointer<SetUserControlActor>(dbSetUserControlActorAddr);

        public static GetPlayerChangeManager dbGetPlayerChangeManagerFunc = Marshal.GetDelegateForFunctionPointer<GetPlayerChangeManager>(dbGetPlayerChangeManagerAddr);
        public static GetActorManagerInstance dbGetActorManagerInstanceFunc = Marshal.GetDelegateForFunctionPointer<GetActorManagerInstance>(dbGetActorManagerInstanceAddr);


        //Original release functions
        public static OnSelectPlayerChangeMenu OnSelectPlayerChangeMenuFunc = Marshal.GetDelegateForFunctionPointer<OnSelectPlayerChangeMenu>(OnSelectPlayerChangeMenuAddr);
        public static GetPlayerChangeManager GetPlayerChangeManagerFunc = Marshal.GetDelegateForFunctionPointer<GetPlayerChangeManager>(GetPlayerChangeManagerAddr);

    }
}
