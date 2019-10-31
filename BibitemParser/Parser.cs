using Genometric.BibitemParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Genometric.BibitemParser
{
    public class Parser<P, A>
        where P : IPublication
        where A : IAuthor
    {
        public Parser(
        IPublicationConstructor<P> publicationConstructor,
        IAuthorConstructor<A> authorConstructor)
        {
            _pubConstructor = publicationConstructor;
            _authorConstructor = authorConstructor;
        }

        private readonly IPublicationConstructor<P> _pubConstructor;
        private readonly IAuthorConstructor<A> _authorConstructor;

        public char[] StopChars = new char[] { '\r', '\n', '\t' };


        public bool TryParse(string bibitem, out P publication)
        {
            publication = default;

            bibitem = RemoveStopChars(bibitem);
            var groups = new Regex(@".*@(?<type>[^{]+){(?<id>[^,]*),(?<body>.+)}").Match(bibitem).Groups;

            // One option could be to fail parsing when `type` is invalid, 
            // but since many bibitems exist with un-specified type, we set 
            // its value to `Unknown` instead of failing parsing process. 
            TryGetType(groups["type"].Value, out BibTexEntryType bibTexEntryType);

            if (!TryGetAttributes(groups["body"].Value, out Dictionary<string, string> attributes))
                return false;

            // To support parsing bibitems without author name, 
            // we do not return false when Author section is not 
            // give, or the given section does not have any 
            // parse-able author name. 
            List<IAuthor> authors = null;
            if (attributes.ContainsKey("author"))
                TryGetAuthors(attributes["author"], out authors);

            publication = _pubConstructor.Construct(
                type: bibTexEntryType,
                doi: attributes.TryGetValue("doi", out string doi) ? doi : null,
                title: attributes.TryGetValue("title", out string title) ? title : null,
                authors: authors,
                date: attributes.TryGetValue("date", out string date) ? date : null,
                year: 1,
                month: 1,
                journal: attributes.TryGetValue("journal", out string journal) ? journal : null,
                volume: attributes.TryGetValue("volume", out string volume) ? int.Parse(volume) : -1,
                number: attributes.TryGetValue("number", out string number) ? int.Parse(number) : -1,
                chapter: attributes.TryGetValue("chapter", out string chapter) ? chapter : null,
                pages: attributes.TryGetValue("pages", out string pages) ? pages : null,
                publisher: attributes.TryGetValue("publisher", out string publisher) ? publisher : null);

            return true;
        }

        private string RemoveStopChars(string bibitem)
        {
            foreach (var c in StopChars)
                bibitem = bibitem.Replace(Convert.ToString(c), string.Empty);
            return bibitem;
        }

        private static bool TryGetType(string input, out BibTexEntryType type)
        {
            if (!Enum.TryParse(input, ignoreCase: true, out type))
            {
                type = BibTexEntryType.Unknown;
                return false;
            }
            else return true;
        }

        private static bool TryGetAttributes(string input, out Dictionary<string, string> attributes)
        {
            attributes = new Dictionary<string, string>();
            MatchCollection matches = Regex.Matches(input, @"(?<a>[^{}]*)\s*=\s*{((?<v>[^{}]+)+)},*");

            if (matches.Count == 0)
                return false;

            foreach (Match match in matches)
                attributes.Add(match.Groups["a"].Value.Trim(), match.Groups["v"].Value.Trim());

            if (attributes.Count == 0)
                return false;

            return true;
        }

        private bool TryGetAuthors(string input, out List<IAuthor> authors)
        {
            // By default authors list is null, hence if no parse-able authors 
            // are determined, then a null list is returned.
            authors = null;
            var names = input.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 0)
                return false;

            authors = new List<IAuthor>();
            foreach (var name in names)
            {
                var sNames = name.Split(',');

                // At least first name should be given in order to create an instance of Author type.
                if (sNames.Length > 0)
                    authors.Add
                        (_authorConstructor.Construct(
                            firstName: sNames.Length > 1 ? sNames[1].Trim() : null,
                            lastName: sNames[0].Trim()));
            }

            return true;
        }
    }
}
