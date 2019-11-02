using System.Collections.Generic;

namespace Genometric.BibitemParser.Interfaces
{
    public interface IPublication<A, K>
        where A : IAuthor
        where K : IKeyword
    {
        public BibTexEntryType Type { get; }
        public string DOI { get; }
        public string Title { get; }
        public ICollection<A> Authors { get; }
        public int? Year { get; }
        public int? Month { get; }
        public string Journal { get; }
        public int? Volume { get; }
        public int? Number { get; }
        public string Chapter { get; }
        public string Pages { get; }
        public string Publisher { get; }
        public ICollection<K> Keywords { get; }
    }
}
