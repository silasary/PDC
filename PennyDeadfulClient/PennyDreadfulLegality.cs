using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PennyDeadfulClient
{
    static class PennyDreadfulLegality
    {
        private static string[] cards = new WebClient().DownloadString("http://pdmtgo.com/legal_cards.txt").Split('\n');

        internal static bool IsLegal(string cardName)
        {
            return cards.Contains(cardName);
        }
    }
}
