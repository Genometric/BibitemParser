using Genometric.BibitemParser.Interfaces;
using System.Collections.Generic;

namespace Genometric.BibitemParser.Model
{
    public class Publication : IPublication<Author, Keyword>
    {
        public string PubMedID { set; get; }

        public string EID { set; get; }

        public string ScopusID { set; get; }

        public BibTexEntryType Type { set; get; }

        public string Title { set; get; }

        public int? Year { set; get; }

        public int? Month { set; get; }

        public int? Day { set; get; }

        public string DOI { set; get; }

        public string BibTeXEntry { set; get; }

        public string Journal { set; get; }

        public string Volume { set; get; }

        public int? Number { set; get; }

        public string Chapter { set; get; }

        public string Pages { set; get; }

        public string Publisher { set; get; }

        public virtual ICollection<Author> Authors { set; get; }

        public virtual ICollection<Keyword> Keywords { set; get; }
    }
}
