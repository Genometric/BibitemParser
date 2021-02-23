using Genometric.BibitemParser.Interfaces;
using Genometric.BibitemParser.Model;

namespace Genometric.BibitemParser.Constructors
{
    public class AuthorConstructor : IAuthorConstructor<Author>
    {
        public Author Construct(string firstName, string lastName)
        {
            return new Author(firstName, lastName);
        }
    }
}
