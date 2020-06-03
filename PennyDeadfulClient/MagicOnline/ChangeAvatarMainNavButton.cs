using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;

namespace PennyDeadfulClient.MagicOnline
{
    class ChangeAvatarMainNavButton
    {
        public string Username;
        private AutomationElement handle;

        public ChangeAvatarMainNavButton(AutomationElement handle)
        {
            this.handle = handle.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "ChangeAvatarMainNavButton"));
            Username = this.handle.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TextBlock")).Current.Name;
        }
    }
}
