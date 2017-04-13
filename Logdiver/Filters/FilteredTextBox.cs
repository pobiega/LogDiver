using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Logdiver.Filters;
using Logdiver.Util;

namespace Logdiver
{
    

    internal class FilteredTextBox : TextBox
    {
        public FilterGroup Filters { get; set; }

        public void OnLine(object sender, LineEventArgs args)
        {

            if (Filters == null || Filters.Matches(args.Line))
                addTextAndScroll(args.Line);
        }

        private void addTextAndScroll(string text)
        {
            AppendText(Environment.NewLine + text);
        }
    }
}
