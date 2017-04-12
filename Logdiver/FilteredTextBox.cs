using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Logdiver.Util;

namespace Logdiver
{
    public enum FilterType
    {
        Contains,
        Regex
    }

    internal class FilteredTextBox : TextBox
    {
        public string Filter { get; set; }
        public Regex FilterRegex { get; set; }
        public FilterType FilterType { get; set; }

        public void OnLine(object sender, LineEventArgs args)
        {
            if (FilterType == FilterType.Contains)
            {
                if (args.Line.Contains(Filter))
                    addTextAndScroll(args.Line);
            }
            else if (FilterType == FilterType.Regex)
            {
                if(FilterRegex == null)
                    FilterRegex = new Regex(Filter);
                if(FilterRegex.IsMatch(args.Line))
                    addTextAndScroll(args.Line);
            }
        }

        private void addTextAndScroll(string text)
        {
            AppendText(Environment.NewLine + text);
        }
    }
}
