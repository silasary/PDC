using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Automation;

namespace PennyDeadfulClient.MagicOnline
{
    partial class PlayLobbySceneView
    {
        public AutomationElement Handle { get; private set; }
        public AutomationElement SelectedEventType { get; private set; }
        public string CurrentEventName { get; set; }
        public AutomationElement PlayLobbyOpenPlayDetails { get; private set; }
        public string DeckName { get; private set; }
        public Decklist Decklist { get; private set; }

        OpenPlayMatch[] Matches;

        internal void Scan(AutomationElement handle)
        {
            this.Handle = handle;
            var EventBlock = handle.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "EventBlocksListBox"));
            SelectedEventType = EventBlock.FindFirst(TreeScope.Children, new PropertyCondition(SelectionItemPattern.IsSelectedProperty, true));
            CurrentEventName = SelectedEventType.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock")).Current.Name;

            PlayLobbyOpenPlayDetails = handle.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "PlayLobbyOpenPlayDetails"));
            
            if (PlayLobbyOpenPlayDetails != null)
            {
                var MatchesListBox = PlayLobbyOpenPlayDetails.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "MatchesListBox"));
                List<OpenPlayMatch> matches = new List<OpenPlayMatch>();
                foreach (AutomationElement openPlayMatch in MatchesListBox.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "ListBoxItem")))
                {
                    matches.Add(new OpenPlayMatch(openPlayMatch));
                }
                Matches = matches.ToArray();
            }

            var PlayLobbyEventDeckView = handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "PlayLobbyEventDeckView"));
            if (PlayLobbyEventDeckView != null)
            {
                DeckName = PlayLobbyEventDeckView.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock")).Current.Name;
            }


            Decklist = new Decklist(handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "PlayLobbyDeckListView")));
            Decklist.IsPDLegal();
        }


        
    }
}
