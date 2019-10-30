using Genometric.BibitemParser.Interfaces;

namespace Genometric.BibitemParser
{
    public class Parser<P, A>
        where P : IPublication
        where A : IAuthor
    {
        public Parser(
        IPublicationConstructor<P> publicationConstructor,
        IAuthorConstructor<A> authorConstructor)
        {
            _pubConstructor = publicationConstructor;
            _authorConstructor = authorConstructor;
        }

        private IPublicationConstructor<P> _pubConstructor;
        private IAuthorConstructor<A> _authorConstructor;


        public bool TryParse(string bibItem, out P publication)
        {
            publication = default(P);
            return true;
        }
    }
}
