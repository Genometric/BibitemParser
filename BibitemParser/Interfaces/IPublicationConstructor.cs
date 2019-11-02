using System.Collections.Generic;

namespace Genometric.BibitemParser.Interfaces
{
    public interface IPublicationConstructor<A, K, out I>
        where A : IAuthor
        where K : IKeyword
        where I : IPublication<A, K>
    {
        I Construct(
            BibTexEntryType type,
            string doi,
            string title,
            List<A> authors,
            int? year,
            int? month,
            string journal,
            int? volume,
            int? number,
            string chapter,
            string pages,
            string publisher,
            List<K> keywords);
    }
}
