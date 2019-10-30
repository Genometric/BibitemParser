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
            string date, 
            int year, 
            int month, 
            string journal, 
            int volume, 
            int number, 
            string chapter, 
            string pages, 
            string publisher)
        {
            return new Publication(
                type, 
                doi, 
                title, 
                authors, 
                date, 
                year, 
                month, 
                journal, 
                volume, 
                number, 
                chapter, 
                pages, 
                publisher);
        }
    }
}
