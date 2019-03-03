using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXVHook
{
    //Black::ActorManager::ActorTypeManager::GetActorFromIndex(Black::ActorManager::ActorTypeManager *this, unsigned int index)
    //Original function from debug build of FFXV

    delegate int GetPartyActorDelegate(uint ptrActorManger, int index);

    public class InjectionEntryPoint : EasyHook.IEntryPoint
    {

        ServerInterface _server = null;
        Queue<String> _messageQueue = new Queue<string>();
        //uint ptrActorTypeManager;
        //int partyIndex;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        IntPtr modBase = GetModuleHandle(null);

        public InjectionEntryPoint(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            _server = EasyHook.RemoteHooking.IpcConnectClient<ServerInterface>(channelName);

        }

        public void Run(EasyHook.RemoteHooking.IContext context, string channelName)
        {
            GetPartyActorDelegate GetPartyActor = Marshal.GetDelegateForFunctionPointer<GetPartyActorDelegate>(modBase + 0xD1D80);
            //GetActorFromIndex(out ptrActorTypeManager, out partyIndex);
            MessageBox.Show((GetPartyActor(0x4ea330, 0x1)).ToString("X"));
            return;
        }
    }
}

