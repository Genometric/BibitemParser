using Genometric.BibitemParser.Interfaces;

namespace Genometric.BibitemParser.UnitTests.Model
{
    public class KeywordConstructor : IKeywordConstructor<Keyword>
    {
        public Keyword Construct(string label)
        {
            return new Keyword(label);
        }
    }
}
