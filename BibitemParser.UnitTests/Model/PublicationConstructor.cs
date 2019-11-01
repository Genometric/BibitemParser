using Genometric.BibitemParser.Interfaces;
using System.Collections.Generic;

namespace Genometric.BibitemParser.UnitTests.Model
{
    public class PublicationConstructor : IPublicationConstructor<Publication>
    {
        public Publication Construct(
            BibTexEntryType type, 
            string doi,
            string title,
            List<IAuthor> authors,
            int? year,
            int? month,
            string journal,
            int? volume,
            int? number,
            string chapter,
            string pages,
            string publisher,
            List<IKeyword> keywords)
        {
            return new Publication(
                type,
                doi,
                title,
                authors,
                year,
                month,
                journal,
                volume,
                number,
                chapter,
                pages,
                publisher,
                keywords);
        }
    }
}
