﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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

            var FFTP = handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Freeform Tournament Practice"));
            if (FFTP == null)
            {
                if (DeckListErrors != null)
                    DeckListErrors = null;
                return;
            }

            var fftpOpenPlayViewModel = TreeWalker.ControlViewWalker.GetParent(TreeWalker.ControlViewWalker.GetParent(FFTP));

            var selctable = fftpOpenPlayViewModel.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            if (!selctable.Current.IsSelected)
            {
                if (DeckListErrors != null)
                    DeckListErrors = null;
                return;
            }
            FftpLabelLocation = FFTP.Current.BoundingRectangle.Location.ToDrawing();

            var PlayLobbyEventDeckView = handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "PlayLobbyEventDeckView"));
            if (PlayLobbyEventDeckView != null && !PlayLobbyEventDeckView.Current.IsOffscreen)
            {
                DeckName = PlayLobbyEventDeckView.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock")).Current.Name;
            }


            AutomationElement PlayLobbyDeckListView = handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "PlayLobbyDeckListView"));
            if (PlayLobbyDeckListView != null)
            {
                Decklist = new Decklist(PlayLobbyDeckListView);
                Decklist.IsPDLegal();
            }
            else if (DeckListErrors != null)
            {
                DeckListErrors = null;
            }

        }
    }
}
