﻿using Genometric.BibitemParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Genometric.BibitemParser
{
    public class Parser<P, A, K>
        where P : IPublication<A, K>
        where A : IAuthor
        where K : IKeyword
    {
        public Parser(
        IPublicationConstructor<A, K, P> publicationConstructor,
        IAuthorConstructor<A> authorConstructor,
        IKeywordConstructor<K> keywordConstructor)
        {
            _pubConstructor = publicationConstructor;
            _authorConstructor = authorConstructor;
            _keywordConstructor = keywordConstructor;

            KeywordsDelimiter = ';';

            BibitemSplitRegex = @".*@(?<type>[^{]+){(?<id>[^,]*),(?<body>.+)}";
            BibitemBodyAttributesRegex = @"(?<attribute>[^{}]*)\s*=\s*\{(?<value>(?:[^{}]|(?<open>\{)|(?<-open>\}))*(?(open)(?!)))\}(,|$)";
        }

        private readonly IPublicationConstructor<A, K, P> _pubConstructor;
        private readonly IAuthorConstructor<A> _authorConstructor;
        private readonly IKeywordConstructor<K> _keywordConstructor;

        /// <summary>
        /// An enumeration of Bibtex fields to be extracted 
        /// from a Bibtex item.
        /// </summary>
        public enum BibtexFields
        {
            Author,
            Title,
            Year,
            Month,
            Day,
            DOI,
            Number,
            Volume,
            Issue,
            Chapter,
            Journal,
            Pages,
            Publisher,
            Keywords
        };

        /// <summary>
        /// Sets and gets an array of characters which the parser 
        /// removes before parsing the input bibitem.
        /// </summary>
        public char[] StopChars = new char[] { '\r', '\n', '\t' };

        /// <summary>
        /// <para>
        /// Sets and gets the regular expression pattern which the parser
        /// uses to extract bibitem type (e.g., Article, Misc; see 
        /// <see cref="BibTexEntryType"/> enum for the supported types) and 
        /// body (e.g., `author = {LastName, FirstName}, title = {Manuscript Title}`). 
        /// </para>
        /// 
        /// <para>The `type` and `body` group names must preserve throughout 
        /// changes to the expression pattern. </para>
        /// 
        /// <para>
        /// The default regular expression matches the patterns such as
        /// the following: 
        /// <list type="bullet">
        /// <item>
        /// <description>Dummy: @article{12, author={LastName1, FirstName1 and LastName2, FirstName2}, journal={IEEE TKDE}, title={Di4}, year={2019}, volume={31}, number={10}, pages={2008-2021}, keywords={indexing;Index structures}, doi={10.1109/TKDE.2018.2871031}, ISSN={}, month={Oct},}</description> 
        /// </item>
        /// <item>
        /// <description>Real: @ARTICLE{8468044, author={Jalili, Vahid and Matteucci, Matteo and Goecks, Jeremy and Deldjoo, Yashar and Ceri, Stefano}, journal={IEEE Transactions on Knowledge and Data Engineering}, title={Next Generation Indexing for Genomic Intervals}, year={2019}, volume={31}, number={10}, pages={2008-2021}, keywords={bioinformatics;data analysis; genetics;genomics;indexing;topology;genomic interval expressions;semantic layer; user-defined function; sense-making;higher-lever reasoning; region-based datasets; logical layer; region calculus; physical layer abstracts;persistence technology; one-dimensional intervals incremental inverted index;trilayer architecture; next generation indexing;topological relations; Di4;bioinformatics application; Bioinformatics;Genomics;Tools;DNA;Indexing;Calculus;Index structures; efficient query processing;genomic data management}, doi={10.1109/TKDE.2018.2871031}, ISSN={}, month={Oct},}</description> 
        /// </item>
        /// </list>
        /// Note that first and last name of authors can be space or comma delimited,
        /// and different author names are separated by ` and `. 
        /// </para>
        /// </summary>
        public string BibitemSplitRegex { set; get; }

        /// <summary>
        /// <para>
        /// Sets and gets the regular expression patterns which the parser
        /// uses to extract attributes (e.g., author names) from the input bibitem body 
        /// (see <see cref="BibitemSplitRegex"/>).
        /// </para>
        /// 
        /// <para>
        /// The `attribute` and `value` group names must preserve throughout 
        /// changes to the expression pattern. 
        /// </para>
        /// 
        /// <para>
        /// An example input to this pattern is as the following:
        /// </para>
        /// <code>
        /// att1={val1}, att2={val2}
        /// </code>
        /// <para>
        /// and this pattern should extract `att1={val1}` and `att2={val2}` 
        /// matches, and separate them as `att1` and `att2` as `attribute` group and
        /// `val1` and `val2` as `value` groups.
        /// </para>
        /// </summary>
        public string BibitemBodyAttributesRegex { set; get; }

        /// <summary>
        /// Sets and gets the delimiter used in keywords. 
        /// By default it is set to `;`. 
        /// </summary>
        public char KeywordsDelimiter { set; get; }


        public bool TryParse(string bibitem, out P publication)
        {
            publication = default;

            bibitem = RemoveStopChars(bibitem);
            var groups = new Regex(BibitemSplitRegex).Match(bibitem).Groups;

            // One option could be to fail parsing when `type` is invalid, 
            // but since many bibitems exist with un-specified type, we set 
            // its value to `Unknown` instead of failing parsing process. 
            TryGetType(groups["type"].Value, out BibTexEntryType bibTexEntryType);

            if (!TryGetAttributes(groups["body"].Value, out Dictionary<BibtexFields, string> attributes))
                return false;

            // To support parsing bibitems without author name, 
            // we do not return false when Author section is not 
            // give, or the given section does not have any 
            // parse-able author name. 
            List<A> authors = null;
            if (attributes.ContainsKey(BibtexFields.Author))
                TryGetAuthors(attributes[BibtexFields.Author], out authors);

            publication = _pubConstructor.Construct(
                type: bibTexEntryType,
                doi: attributes.TryGetValue(BibtexFields.DOI, out string doi) ? FormatDOI(doi) : null,
                title: attributes.TryGetValue(BibtexFields.Title, out string title) ? title.Trim() : null,
                authors: authors,
                year: TryGetNullableInt(attributes, BibtexFields.Year),
                month: TryGetNullableInt(attributes, BibtexFields.Month),
                day: TryGetNullableInt(attributes, BibtexFields.Day),
                journal: attributes.TryGetValue(BibtexFields.Journal, out string journal) ? journal.Trim() : null,
                volume: attributes.TryGetValue(BibtexFields.Volume, out string vol) ? vol.Trim() : null,
                number: TryGetNullableInt(attributes, BibtexFields.Number),
                chapter: attributes.TryGetValue(BibtexFields.Chapter, out string chapter) ? chapter.Trim() : null,
                pages: attributes.TryGetValue(BibtexFields.Pages, out string pages) ? pages.Trim() : null,
                publisher: attributes.TryGetValue(BibtexFields.Publisher, out string publisher) ? publisher.Trim() : null,
                keywords: TryGetKeywords(attributes, out List<K> keywords) ? keywords : null);

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

        private static string FormatDOI(string doi)
        {
            doi = doi.Replace("doi:", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            doi = doi.Replace("http://doi.org/", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            doi = doi.Replace("https://doi.org/", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            doi = doi.Replace("http://dx.doi.org/", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            doi = doi.Replace("https://dx.doi.org/", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            if (string.Equals(doi, "na", StringComparison.InvariantCultureIgnoreCase))
                return null;
            else
                return doi;
        }

        private bool TryGetAttributes(string input, out Dictionary<BibtexFields, string> attributes)
        {
            attributes = new Dictionary<BibtexFields, string>();
            MatchCollection matches = Regex.Matches(input, BibitemBodyAttributesRegex);

            if (matches.Count == 0)
                return false;

            foreach (Match match in matches)
            {
                if (!Enum.TryParse(match.Groups["attribute"].Value.Trim(), true, out BibtexFields attribute))
                    continue;

                // Not removing `{` and `}` from author names because
                // they can start or end with _accent_ characters which 
                // can be provided using LaTeX syntax (e.g., "{\~e}").
                // However, this approach is not universal, because 
                // (a) it does not apply to other fields such as title,
                // and (b) it does not differentiate between formatting
                // and non-formatting brackets. For instance:
                // - Should remove brackets: {abc}
                // - Should not remove brackets: {\"e}fg 
                char[] stopChars;
                if (attribute == BibtexFields.Author)
                    stopChars = new char[] { ' ' };
                else
                    stopChars = new char[] { ' ', '{', '}' };

                attributes.Add(
                    attribute,
                    match.Groups["value"].Value.Trim(stopChars));
            }

            if (attributes.Count == 0)
                return false;

            return true;
        }

        private int? TryGetNullableInt(Dictionary<BibtexFields, string> attributes, BibtexFields field)
        {
            if (attributes.TryGetValue(field, out string v))
            {
                if (int.TryParse(v, out int r))
                    return r;
                else if (field == BibtexFields.Month)
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
            else if (field == BibtexFields.Number)
            {
                return TryGetNullableInt(attributes, BibtexFields.Issue);
            }
            return null;
        }

        private bool TryGetAuthors(string input, out List<A> authors)
        {
            // By default authors list is null, hence if no parse-able authors 
            // are determined, then a null list is returned.
            authors = null;
            var names = input.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 0)
                return false;

            authors = new List<A>();
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

        private bool TryGetKeywords(Dictionary<BibtexFields, string> attributes, out List<K> keywords)
        {
            keywords = null;
            if (attributes.TryGetValue(BibtexFields.Keywords, out string input))
            {
                var words = input.Split(KeywordsDelimiter);
                if (words.Length == 0)
                    return false;

                keywords = new List<K>();
                foreach (var word in words)
                    keywords.Add(_keywordConstructor.Construct(word.Trim()));
                return true;
            }
            return false;
        }
    }
}
