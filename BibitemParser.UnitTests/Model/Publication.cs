﻿using Genometric.BibitemParser.Interfaces;
using System.Collections.Generic;

namespace Genometric.BibitemParser.UnitTests.Model
{
    public class Publication : IPublication<Author, Keyword>
    {
        public BibTexEntryType Type { get; }
        public string DOI { get; }
        public string Title { get; }
        public int? Year { get; }
        public int? Month { get; }
        public int? Day { get; }
        public string Journal { get; }
        public string Volume { get; }
        public int? Number { get; }
        public string Chapter { get; }
        public string Pages { get; }
        public string Publisher { get; }
        public ICollection<Author> Authors { get; }
        public ICollection<Keyword> Keywords { get; }

        public Publication(
            BibTexEntryType type, 
            string doi, 
            string title,
            ICollection<Author> authors,
            int? year,
            int? month,
            int? day,
            string journal, 
            string volume, 
            int? number, 
            string chapter, 
            string pages, 
            string publisher,
            ICollection<Keyword> keywords)
        {
            Type = type;
            DOI = doi;
            Title = title;
            Authors = authors;
            Year = year;
            Month = month;
            Day = day;
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
