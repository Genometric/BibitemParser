using Genometric.BibitemParser.Interfaces;
using Genometric.BibitemParser.Model;
using System.Collections.Generic;

namespace Genometric.BibitemParser.Constructors
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
            string volume,
            int? number,
            string chapter,
            string pages,
            string publisher,
            List<Keyword> keywords)
        {
            return new Publication()
            {
                Type = type,
                DOI = doi,
                Title = title,
                Authors = authors,
                Year = year,
                Month = month,
                Day = day,
                Journal = journal,
                Volume = volume,
                Number = number,
                Chapter = chapter,
                Pages = pages,
                Publisher = publisher,
                Keywords = keywords
            };
        }
    }
}
