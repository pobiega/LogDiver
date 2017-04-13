using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logdiver.Filters
{
    public class Filter
    {
        [DisplayName("Filter type")]
        public FilterType FilterType { get; set; }

        private string _filterText;
        [DisplayName("Filter text")]
        public string FilterText {
            get { return _filterText; }
            set
            {
                if(FilterType == FilterType.Regex)
                    FilterRegex = new Regex(value);
                _filterText = value;
            }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public Regex FilterRegex { get; private set; }

        public Filter()
        { }

        public Filter(FilterType type, string text)
        {
            this.FilterType = type;
            this.FilterText = text;
        }

        public bool Matches(string text)
        {
            return FilterType == FilterType.Contains ? text.Contains(FilterText) : FilterRegex.IsMatch(text);
        }

        public override string ToString()
        {
            return $"{FilterType.ToString()} {FilterText}";
        }
    }
}
