using Genometric.BibitemParser.Interfaces;
using System.Collections.Generic;

namespace Genometric.BibitemParser.UnitTests.Model
{
    public class Publication : IPublication
    {
        public BibTexEntryType Type { get; }
        public string DOI { get; }
        public string Title { get; }
        public ICollection<IAuthor> Authors { get; }
        public int? Year { get; }
        public int? Month { get; }
        public string Journal { get; }
        public int? Volume { get; }
        public int? Number { get; }
        public string Chapter { get; }
        public string Pages { get; }
        public string Publisher { get; }
        public ICollection<IKeyword> Keywords { get; }

        public Publication(
            BibTexEntryType type, 
            string doi, 
            string title,
            ICollection<IAuthor> authors,
            int? year,
            int? month, 
            string journal, 
            int? volume, 
            int? number, 
            string chapter, 
            string pages, 
            string publisher,
            ICollection<IKeyword> keywords)
        {
            Type = type;
            DOI = doi;
            Title = title;
            Authors = authors;
            Year = year;
            Month = month;
            Journal = journal;
            Volume = volume;
            Number = number;
            Chapter = chapter;
            Pages = pages;
            Publisher = publisher;
            Keywords = keywords;
        }
    }
}
