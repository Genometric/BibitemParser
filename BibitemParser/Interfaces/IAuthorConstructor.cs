namespace Genometric.BibitemParser.Interfaces
{
    public interface IAuthorConstructor<out I>
        where I : IAuthor
    {
        I Construct(
            string firstName, 
            string lastName);
    }
}
