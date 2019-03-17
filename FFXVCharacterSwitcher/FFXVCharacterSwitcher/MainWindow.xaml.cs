using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace FFXVCharacterSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FFXVHook.ServerInterface server;
        private string channelName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PartyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PartyButton_Click(object sender, RoutedEventArgs e)
        {
            int index = PartyBox.SelectedIndex;

            //server = EasyHook.RemoteHooking.IpcConnectClient<FFXVHook.ServerInterface>(channelName);

            server?.SwitchCharacter(index);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void CheckBoxUnchecked(object send, RoutedEventArgs e)
        {
            
        }

        private void InjectHacks_Click(object sender, RoutedEventArgs e)
        {
            //its time
            Int32 targetPID = 0;
            string targetexe = "ffxv_s";
            channelName = null;

            try
            {
                Process process = Process.GetProcessesByName(targetexe)[0];
                targetPID = process.Id;
            }

            catch
            {
                MessageBox.Show("Error: Game Not Found!");
                return;
            }

            //Create IPC server from FFXVHook dll
            EasyHook.RemoteHooking.IpcCreateServer<FFXVHook.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
            Console.WriteLine(channelName);

            //Path to assembly to inject into FFXV
            string InjectionLibrary = @System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "FFXVHook.dll");
            Console.WriteLine(InjectionLibrary);

            EasyHook.RemoteHooking.Inject(targetPID, InjectionLibrary, InjectionLibrary, channelName);
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            server = EasyHook.RemoteHooking.IpcConnectClient<FFXVHook.ServerInterface>(channelName);

            if (server != null) server.Disconnect();
        }
    }
}
