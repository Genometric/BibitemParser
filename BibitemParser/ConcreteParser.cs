using Genometric.BibitemParser.Constructors;
using Genometric.BibitemParser.Model;

namespace Genometric.BibitemParser
{
    public class Parser : Parser<Publication, Author, Keyword>
    {
        public Parser() : base(
            new PublicationConstructor(),
            new AuthorConstructor(),
            new KeywordConstructor())
        { }
    }
}
