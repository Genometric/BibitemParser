using Genometric.BibitemParser.Interfaces;

namespace Genometric.BibitemParser.UnitTests.Model
{
    public class AuthorConstructor : IAuthorConstructor<Author>
    {
        public Author Construct(string firstName, string lastName)
        {
            return new Author(firstName, lastName);
        }
    }
}
