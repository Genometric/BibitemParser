using System.Collections.Generic;

namespace Genometric.BibitemParser.Interfaces
{
    public interface IPublicationConstructor<out I>
        where I : IPublication
    {
        I Construct(
            BibTexEntryType type,
            string doi,
            string title,
            List<IAuthor> authors,
            string date,
            int year,
            int month,
            string journal,
            int volume,
            int number,
            string chapter,
            string pages,
            string publisher);
    }
}
