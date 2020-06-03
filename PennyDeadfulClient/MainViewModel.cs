using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Threading;

namespace PennyDeadfulClient
{
    class MainViewModel : BaseNotifyPropertyChanged
    {
        static SettingsManager<Settings> SettingsManager { get; } = new SettingsManager<Settings>("config.json");
        public static Settings Settings { get; private set; } = SettingsManager.LoadSettings() ?? new Settings();
        public string Caption { get => _caption; private set => SetField(ref _caption, value); }

        MagicOnline.ShinyMainView shinyMain;
        private string _caption = "Loading...";

        private DispatcherTimer timer;

        Discord.Discord Discord { get; } = new Discord.Discord(338056190779195392, (ulong)CreateFlags.Default);

        public MainViewModel()
        {
            shinyMain = new MagicOnline.ShinyMainView();
            shinyMain.PropertyChanged += ShinyMain_PropertyChanged;
            shinyMain.Scan();
            if (shinyMain.CurrentHandle == null)
                SetCaption("Please launch MTGO");
        }

        private void SetCaption(string text)
        {
            Caption = text;
            (Discord.ActivityManagerInstance ?? Discord.GetActivityManager()).UpdateActivity(new Activity { State = text }, OnActivity);
        }

        private void OnActivity(Result result)
        {
        }

        internal void SetTimer(Dispatcher dispatcher)
        {
            timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnCapture, dispatcher);
            timer.Start();
        }

        private void OnCapture(object sender, EventArgs e)
        {
            shinyMain.Scan();
            //Discord.RunCallbacks();
        }

        private void ShinyMain_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Username")
            {
                if (string.IsNullOrEmpty(shinyMain.Username))
                    SetCaption("Not logged into MTGO");
                else
                    SetCaption($"Logged in as {shinyMain.Username}");
            }
            //if (e.PropertyName == "CurrentScene")
            //{
            //    Console.WriteLine(shinyMain.CurrentScene.Current.ClassName);
            //}
        }
    }
}
