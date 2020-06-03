using System;
using System.Collections.Generic;
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
            foreach (AutomationElement item in automationElement.FindAll(TreeScope.Children, Condition.TrueCondition))
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
            bool allLegal = true;
            foreach (var card in Maindeck.Union(Sideboard))
            {
                bool legal = PennyDreadfulLegality.IsLegal(card.CardName);
                if (!legal)
                {
                    allLegal = false;
                    if (!card.handle.Current.IsOffscreen)
                    {
                        // Overlay
                        var loc = card.handle.Current.BoundingRectangle;
                        
                    }
                }
            }
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

            }

            public int Quantity { get; }
            public string CardName { get; }
            
            public override string ToString()
            {
                return $"{Quantity} {CardName}";
            }
        }
    }
}