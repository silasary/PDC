using System.Windows.Automation;
using System.Linq;

namespace PennyDeadfulClient.MagicOnline
{
    internal class OpenPlayMatch
    {
        private AutomationElement openPlayMatch;
        public string Action { get; }
        public string Comment { get; }
        public string[] Players { get; }

        public OpenPlayMatch(AutomationElement openPlayMatch)
        {
            this.openPlayMatch = openPlayMatch;
            var children = openPlayMatch.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock")).Cast<AutomationElement>().ToList();
            if (!children.Any())
                return;
            Action = children.Last().Current.Name;
            children.RemoveAt(children.Count - 1);
            Comment = children.Last().Current.Name;
            children.RemoveAt(children.Count - 1);
            Players = children.Select(p => p.Current.Name).ToArray();
        }
    }
}