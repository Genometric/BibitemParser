using Genometric.BibitemParser.Interfaces;

namespace Genometric.BibitemParser.UnitTests.Model
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
