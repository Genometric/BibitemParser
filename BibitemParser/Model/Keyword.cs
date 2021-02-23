using Genometric.BibitemParser.Interfaces;

namespace Genometric.BibitemParser.Model
{
    public class Keyword : IKeyword
    {
        public string Label { get; }

        public Keyword(string label)
        {
            Label = label;
        }
    }
}
