namespace Genometric.BibitemParser.Interfaces
{
    public interface IKeywordConstructor<out I>
        where I : IKeyword
    {
        I Construct(string label);
    }
}
