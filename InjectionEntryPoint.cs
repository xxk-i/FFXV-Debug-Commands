using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFXVCharacterSwitcher
{
    //Black::ActorManager::ActorTypeManager::GetActorFromIndex(Black::ActorManager::ActorTypeManager *this, unsigned int index)
    //Original function from debug build of FFXV
    [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
    delegate void GetActorFromIndexDelegate(uint ptrActorTypeManger, int index);

    public class InjectionEntryPoint : EasyHook.IEntryPoint
    {

        ServerInterface _server = null;
        Queue<String> _messageQueue = new Queue<string>();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        IntPtr modBase = GetModuleHandle(null);

        public InjectionEntryPoint(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            _server = EasyHook.RemoteHooking.IpcConnectClient<ServerInterface>(channelName);
            
        }

        public void Run(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            Marshal.GetDelegateForFunctionPointer<GetActorFromIndexDelegate>(modBase + 0xDF850);
        }
    }
}

