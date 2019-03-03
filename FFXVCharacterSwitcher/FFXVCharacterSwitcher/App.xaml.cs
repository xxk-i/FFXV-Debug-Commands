using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using EasyHook;
using System.Runtime.InteropServices;

namespace FFXVCharacterSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //its time
        Int32 targetPID = 0;
        string targetexe = "FINAL FANTASY XV WINDOWS EDITION (build 1138403)";
        string channelName = null;

        void App_Startup(object sender, StartupEventArgs e)
        {
            EasyHook.RemoteHooking.IpcCreateServer<FFXVHook.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

            string in
        }
        
    }
}
