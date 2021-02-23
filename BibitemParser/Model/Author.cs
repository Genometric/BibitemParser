using Genometric.BibitemParser.Interfaces;

namespace Genometric.BibitemParser.Model
{
    public class Author : IAuthor
    {
        public string FirstName { get; }
        public string LastName { get; }

        public Author(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
