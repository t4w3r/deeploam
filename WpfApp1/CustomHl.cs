using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace WpfApp1
{
    class CustomHl : ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition
    {

        public CustomHl(string name)
        {
            HighlightingRuleSet rs = new HighlightingRuleSet();
            
            Properties.Add("HTML","HTML" );
            Name = "HTML";
        }

        public string Name { get; }
        //
        // Summary:
        //     Gets the main rule set.
        public HighlightingRuleSet MainRuleSet { get; }
        //
        // Summary:
        //     Gets the list of named highlighting colors.
        public IEnumerable<HighlightingColor> NamedHighlightingColors { get; }
        //
        // Summary:
        //     Gets the list of properties.
        public IDictionary<string, string> Properties { get; }

        //
        // Summary:
        //     Gets a named highlighting color.
        //
        // Returns:
        //     The highlighting color, or null if it is not found.
        public HighlightingColor GetNamedColor(string name)
        {
            return null;
        }
        //
        // Summary:
        //     Gets a rule set by name.
        //
        // Returns:
        //     The rule set, or null if it is not found.
        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            return MainRuleSet;
        }
    }

}

