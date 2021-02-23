using Genometric.BibitemParser.Interfaces;
using Genometric.BibitemParser.Model;

namespace Genometric.BibitemParser.Constructors
{
    public class KeywordConstructor : IKeywordConstructor<Keyword>
    {
        public Keyword Construct(string label)
        {
            return new Keyword(label);
        }
    }
}
