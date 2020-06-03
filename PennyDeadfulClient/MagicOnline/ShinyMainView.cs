using DSharpPlus.Net;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Automation;

namespace PennyDeadfulClient.MagicOnline
{
    class ShinyMainView : BaseNotifyPropertyChanged
    {
        public AutomationElement CurrentHandle { get; private set; }

        System.Diagnostics.Process GetProcess() => System.Diagnostics.Process.GetProcessesByName("mtgo").FirstOrDefault();

        AutomationElement GetHandle()
        {
            System.Diagnostics.Process process = GetProcess();
            if (process == null)
            {
                CurrentHandle = null;
                return null;
            }

            return CurrentHandle = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                    new PropertyCondition(AutomationElement.ClassNameProperty, "Window"),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "ShinyMainView")));
        }

        public void Scan()
        {
            var handle = GetHandle();
            if (handle == null)
                return;

            if (string.IsNullOrEmpty(Username))
                Username = new ChangeAvatarMainNavButton(handle).Username;

            var RadPaneGroup = handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "MainWindowPane"));
            CurrentScene = RadPaneGroup.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "custom"));
            switch (currentScene.Current.ClassName)
            {
                case "PlayLobbySceneView":
                    if (PlayLobby == null)
                    {
                        this.PlayLobby = new PlayLobbySceneView();
                        PlayLobby.StartOverlay(GetProcess());
                    }

                    this.PlayLobby.Scan(CurrentScene);
                    break;
                default:
                    break;
            }

        }

        string username;
        public string Username
        {
            get => username;
            set => SetField(ref username, value);
        }
        AutomationElement currentScene;
        public AutomationElement CurrentScene { get => currentScene; private set => SetField(ref currentScene, value); }
        public PlayLobbySceneView PlayLobby { get; private set; }
    }
}
