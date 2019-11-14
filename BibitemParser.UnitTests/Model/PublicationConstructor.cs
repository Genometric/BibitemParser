using Genometric.BibitemParser.Interfaces;
using System.Collections.Generic;

namespace Genometric.BibitemParser.UnitTests.Model
{
    public class PublicationConstructor : IPublicationConstructor<Author, Keyword, Publication>
    {
        public Publication Construct(
            BibTexEntryType type,
            string doi,
            string title,
            List<Author> authors,
            int? year,
            int? month,
            int? day,
            string journal,
            int? volume,
            int? number,
            string chapter,
            string pages,
            string publisher,
            List<Keyword> keywords)
        {
            return new Publication(
                type,
                doi,
                title,
                authors,
                year,
                month,
                day,
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
