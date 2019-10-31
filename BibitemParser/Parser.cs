using Genometric.BibitemParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            ;
            publication = _pubConstructor.Construct(
                type: bibTexEntryType,
                doi: attributes.TryGetValue("doi", out string doi) ? doi.Trim() : null,
                title: attributes.TryGetValue("title", out string title) ? title.Trim() : null,
                authors: authors,
                year: TryGetNullableInt(attributes, "year"),
                month: TryGetNullableInt(attributes, "month"),
                journal: attributes.TryGetValue("journal", out string journal) ? journal.Trim() : null,
                volume: TryGetNullableInt(attributes, "volume"),
                number: TryGetNullableInt(attributes, "number"),
                chapter: attributes.TryGetValue("chapter", out string chapter) ? chapter.Trim() : null,
                pages: attributes.TryGetValue("pages", out string pages) ? pages.Trim() : null,
                publisher: attributes.TryGetValue("publisher", out string publisher) ? publisher.Trim() : null);

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

        private int? TryGetNullableInt(Dictionary<string, string> attributes, string input)
        {
            if (attributes.TryGetValue(input, out string v))
            {
                if (int.TryParse(v, out int r))
                    return r;
                else if (input == "month")
                {
                    if (DateTime.TryParseExact(
                        v, "MMMM",
                        CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal,
                        out DateTime month))
                        return month.Month;
                    else if (DateTime.TryParseExact(
                        v, "MMM",
                        CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal,
                        out month))
                        return month.Month;
                }
            }
            else if (input == "number")
            {
                return TryGetNullableInt(attributes, "issue");
            }
            return null;
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
                // Sometimes first and last names are separated using ','. 
                // Hence, if ',' exist, we assume it is the delimiter.
                // This is a loose assumption, hence this part needs to 
                // be improved. 
                string[] sNames = null;
                if (name.Contains(','))
                    sNames = name.Split(',');
                else
                    sNames = name.Split(' ');

                // At least last name should be given in order to create an instance of Author type.
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
