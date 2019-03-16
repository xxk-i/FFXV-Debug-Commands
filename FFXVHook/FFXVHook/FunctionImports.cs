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

        //Refers to in battle character switching menu, can also be used to switch to unlisted index 4 (guest)
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void OnSelectPlayerChangeMenu(UInt64 ptrPlayerChangeManager, int index);

        //SQEX::Luminous::Core::Singleton<Black::System::PlayerChange::PlayerChangeManager>::Get()
        //Returns PlayerChangeManager instance (pointer) to use for its class member functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate UInt64 GetPlayerChangeManager();

        //Pointers
        //For debug, seems to be IDA file offset + C00
        public static IntPtr OnSelectPlayerChangeMenuAddr = (modBase + 0x8BEF10);
        public static IntPtr GetPlayerChangeManagerFuncAddr = (modBase + 0xC8CA0);

        //Original functions
        public static OnSelectPlayerChangeMenu OnSelectPlayerChangeMenuFunc = Marshal.GetDelegateForFunctionPointer<OnSelectPlayerChangeMenu>(OnSelectPlayerChangeMenuAddr);
        public static GetPlayerChangeManager GetPlayerChangeManagerFunc = Marshal.GetDelegateForFunctionPointer<GetPlayerChangeManager>(GetPlayerChangeManagerFuncAddr);
    }
}
