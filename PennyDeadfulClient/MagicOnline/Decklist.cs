﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace PennyDeadfulClient.MagicOnline
{
    internal class Decklist
    {
        private LineItem[] Maindeck { get; }
        private LineItem[] Sideboard { get; }
        private AutomationElement automationElement;

        public Decklist(AutomationElement automationElement)
        {
            var main = new List<LineItem>();
            var sb = new List<LineItem>();
            List<LineItem> curr = null;
            this.automationElement = automationElement;
            foreach (AutomationElement item in automationElement.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition))
            {
                switch (item.Current.ClassName)
                {
                    case "TabControl":
                    case "ScrollBar":
                        break;
                    case "TextBlock":
                        if (item.Current.Name.StartsWith("MAIN DECK"))
                            curr = main;
                        else if (item.Current.Name.StartsWith("SIDEBOARD"))
                            curr = sb;
                        else
                            break;
                        break;
                    case "PlayLobbyDeckItemView":
                        curr.Add(new LineItem(item));
                        break;
                    default:
                        break;
                }
            }
            Maindeck = main.ToArray();
            Sideboard = sb.ToArray();
        }

        public string DecklistTxt => string.Concat(string.Join('\n', Maindeck.Select(i => i.ToString())), "\n\n", string.Join('\n', Sideboard.Select(i => i.ToString())));

        public bool IsPDLegal()
        {
            var decklistErrors = new List<Point>();
            bool allLegal = true;
            foreach (var card in Maindeck.Union(Sideboard))
            {
                if (!card.Legal)
                {
                    allLegal = false;
                    if (!card.handle.Current.IsOffscreen)
                    {
                        // Overlay
                        var loc = card.handle.Current.BoundingRectangle;
                        decklistErrors.Add(new Point((int)loc.X, (int)loc.Y));
                    }
                }
            }
            PlayLobbySceneView.DeckListErrors = decklistErrors.ToArray();

            return allLegal;
        }

        private class LineItem
        {
            internal AutomationElement handle;

            public LineItem(AutomationElement handle)
            {
                this.handle = handle;
                var texts = handle.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock")).Cast<AutomationElement>().ToArray();
                this.Quantity = int.Parse(texts[0].Current.Name);
                this.CardName = texts[1].Current.Name;
                Legal = PennyDreadfulLegality.IsLegal(CardName); 
            }

            public int Quantity { get; }
            public string CardName { get; }

            public bool Legal { get; set; }

            public override string ToString()
            {
                return $"{Quantity} {CardName}";
            }
        }
    }
}