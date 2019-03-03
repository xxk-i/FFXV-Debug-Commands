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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void partyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void partyButton_Click(object sender, RoutedEventArgs e)
        {
            int selection = PartyBox.SelectedIndex;

            Switcher.SwitchToPartyMember(PartyBox.SelectedIndex);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            //partyButton.SetValue(
        }

        private void checkBoxUnchecked(object send, RoutedEventArgs e)
        {
            //partyBox.IsReadOnly = false;
        }

        private void noctisPointerButton_Click(object sender, RoutedEventArgs e)
        {
            //its time
            Int32 targetPID = 0;
            string targetexe = "ffxv_s";
            string channelName = null;

            Process process = Process.GetProcessesByName(targetexe)[0];
            targetPID = process.Id;

            //Create IPC server from FFXVHook dll
            EasyHook.RemoteHooking.IpcCreateServer<FFXVHook.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

            //Path to assembly to inject into FFXV
            //string InjectionLibrary = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "FFXVHook.dll");
            //"C:\\Users\\jeremy\\Desktop\\ffxv-character-switcher\\FFXVCharacterSwitcher\\FFXVCharacterSwitcher\\bin\\Debug\\FFXVHook.dll"
            string InjectionLibrary = @"C:\Users\jeremy\Desktop\FFXVHook.dll";

            EasyHook.RemoteHooking.Inject(targetPID, InjectionLibrary, InjectionLibrary, channelName);
        }
    }
}
