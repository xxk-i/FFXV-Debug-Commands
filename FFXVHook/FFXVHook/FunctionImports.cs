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

        //Black::ActorManager::ActorTypeManager::GetActorFromIndex(Black::ActorManager::ActorTypeManager *this, unsigned int index)
        //Original function from debug build of FFXV
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate void OnSelectPlayerChangeMenuDelegate(UInt64 ptrPlayerChangeManager, int index);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate IntPtr GetPlayerChangeManager();
        
        public delegate void OnSelectPlayerChangeMenuOriginal(UInt64 ptrPlayerChangeManager, int index); //get *this from SQEX::Luminous::Core::Singleton<Black::System::PlayerChange::PlayerChangeManager>::Get()

        //Pointers
        public static IntPtr OnSelectPlayerChangeMenuAddr = (modBase + 0x8BEF10);

        //Original functions
        public static OnSelectPlayerChangeMenuOriginal OnSelectPlayerChangeMenuFunc = Marshal.GetDelegateForFunctionPointer<OnSelectPlayerChangeMenuOriginal>(OnSelectPlayerChangeMenuAddr);
    }
}
