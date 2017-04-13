using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Logdiver.Filters
{
    public class FilterGroup
    {
        public FilterGroup() : this(FilterStrategy.Any) { }

        public FilterGroup(FilterStrategy strategy)
        {
            this.Strategy = strategy;
            Filters = new List<Filter>();
        }

        [DisplayName("Filters")]
        public List<Filter> Filters { get; set; }

        [DisplayName("Strategy")]
        public FilterStrategy Strategy { get; set; }

        public bool Matches(string line)
        {
            if (Filters == null)
                return true;

            if (Strategy == FilterStrategy.Any)
            {
                if (Filters.Any(filter => filter.Matches(line)))
                {
                    return true;
                }
            }
            else
            {
                if (Filters.All(filter => filter.Matches(line)))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            if (Filters == null || Filters.Count == 0)
                return "Blank filter";

            var sb = new StringBuilder();

            foreach (var filter in Filters)
                sb.Append($"{filter.ToString()}, ");

            return sb.ToString().Trim().TrimEnd(',');
        }
    }
}
