using System.Collections.Generic;

namespace Genometric.BibitemParser.Interfaces
{
    public interface IPublication
    {
        public BibTexEntryType Type { get; }
        public string DOI { get; }
        public string Title { get; }
        public List<IAuthor> Authors { get; }
        public int? Year { get; }
        public int? Month { get; }
        public string Journal { get; }
        public int? Volume { get; }
        public int? Number { get; }
        public string Chapter { get; }
        public string Pages { get; }
        public string Publisher { get; }
        public List<IKeyword> Keywords { get; }
    }
}
