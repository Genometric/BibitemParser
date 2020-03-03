using Genometric.BibitemParser.UnitTests.Model;
using System.Collections.Generic;
using Xunit;

namespace Genometric.BibitemParser.UnitTests
{
    public class ExtractFields
    {
        private readonly Parser<Publication, Author, Keyword> _parser;

        private const string bibitemS00 = "@article{ID, author = {lName fName}, title = {A: title; on, some---topic}, journal = {journaltitle}, year = {2019}, doi={DoI:10.1109/TKDE.2018.2871031}, number={21}, chapter={a chapter in the journal}, keywords={akeyword}}";
        private const string bibitemS01 = "@article{id, title={{a_title}}, author={lname1, fname1 and lname{{\\\"e}}, fname2 and lname3, fname{\\\"3}}, doi={{10.1109/TKDE.2018.2871031}}, journal={{journal name}}, publisher={{publisher}}, volume = {{10}}, issue = {5}, pages = {123-456}}";
        private const string bibitemS02 = "@misc{this_is_my_id, author = {first_name1, m.lastname1 and firstname_2, lastname2}, title = {and a title }, doi={{DOI:10.1109/TKDE.2018.2871031}}, year = {2020}, pages={100--101}, keywords={firstkeyword, secondkeyword }}";
        private const string bibitemS03 = "@misc{this_is_my_id, author = {first_name1, m.lastname1 and firstname_2, lastname2}, title = {and a title }, DOI={HTTP://doi.org/10.1109/TKDE.2018.2871031}, year = {2020}, pages={100--101}, keywords={firstkeyword, secondkeyword }}";
        private const string bibitemS04 = "@Manual{,title = {my: title},author = {A. lName and B. lName},year = {2019},note = {I am optional},url = {https://https://genometric.github.io/MSPC/}, pages={ A001-6}, doi={NA}, keywords={firstkeyword; secondkeyword}, volume = {11-A}}";
        private const string bibitemS05 = "@PhdThesis{,title = {my thesis title},author = {V J and A B. C and firstName LastName},url = {https://github.com/Genometric/MSPC/},doi = {http://dx.doi.org/10.1109/TKDE.2018.2871031},school = {polimi},year = {2016},}";
        private const string bibitemS06 = "@Book{,title = {MSPC, CPSM},year = {2019},author = {V B. J and otherFirstName C. otherLastName}, doi = {https://doi.org/10.1109/TKDE.2018.2871031}, publisher = {somepublisher},month = {9}, chapter=  {chapter in the book}}";
        private const string bibitemS07 = "@TechReport{,author = {abc efg and hjk lmn},title = {I am a tec rep.},institution = {xyz, Dep. rnd},year = {2020},doi = {https://dx.doi.org/10.1109/TKDE.2018.2871031},type = {Technical report}, month = {December},}";
        private const string bibitemS08 = "@cannotguess{,author = {hmmm haaa},title = {aaa: bbb (ccc)},year = {2019}, month = {Aug}, day={1}}";

        // This sample has line breaks (i.e., the '\r' and '\n' characters) 
        // and tabs (i.e., the '\t' character); hence can assert if parser can 
        // still parse data while such characters are present. 
        private const string bibitemS09 =
            @"@book{ID,
	author = {fName M. lName},
	title = {sometitle},
	year = {2019},
}";

        private const string bibitemS10 =
            @"@ARTICLE{8468044,
author={Jalili, Vahid and Matteucci, Matteo and Goecks, Jeremy and Deldjoo, Yashar and Ceri, Stefano},
journal={IEEE Transactions on Knowledge and Data Engineering},
title={Next Generation Indexing for Genomic Intervals},
year={2019},
volume={31},
number={10},
pages={2008-2021},
keywords={bioinformatics;data analysis;genetics;genomics;indexing;topology;genomic interval expressions;semantic layer;user-defined function;sense-making;higher-lever reasoning;region-based datasets;logical layer;region calculus;physical layer abstracts;persistence technology;one-dimensional intervals incremental inverted index;trilayer architecture;next generation indexing;topological relations;Di4;bioinformatics application;Bioinformatics;Genomics;Tools;DNA;Indexing;Calculus;Index structures;efficient query processing;genomic data management},
doi={10.1109/TKDE.2018.2871031},
ISSN={},
month={Oct},}";

        public ExtractFields()
        {
            _parser = new Parser<Publication, Author, Keyword>(
                new PublicationConstructor(),
                new AuthorConstructor(),
                new KeywordConstructor());
        }

        [Fact]
        public void CompleteExample()
        {
            // Arrange
            var names = new List<string[]>
            {
                new string[] {"Vahid",   "Jalili"},
                new string[] {"Matteo",  "Matteucci"},
                new string[] {"Jeremy",  "Goecks"},
                new string[] {"Yashar",  "Deldjoo"},
                new string[] {"Stefano", "Ceri"}
            };

            var keywords = new List<string>
            {
                "bioinformatics", "data analysis", "genetics", "genomics",
                "indexing", "topology", "genomic interval expressions",
                "semantic layer", "user-defined function", "sense-making",
                "higher-lever reasoning", "region-based datasets", "logical layer",
                "region calculus", "physical layer abstracts", "persistence technology",
                "one-dimensional intervals incremental inverted index", "trilayer architecture",
                "next generation indexing", "topological relations", "Di4",
                "bioinformatics application", "Bioinformatics", "Genomics", "Tools",
                "DNA", "Indexing", "Calculus", "Index structures",
                "efficient query processing", "genomic data management"
            };

            // Act
            var success = _parser.TryParse(bibitemS10, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(BibTexEntryType.Article, pub.Type);
            foreach (var name in names)
                Assert.Contains(pub.Authors, a => a.FirstName == name[0] && a.LastName == name[1]);
            Assert.Equal("IEEE Transactions on Knowledge and Data Engineering", pub.Journal);
            Assert.Equal("Next Generation Indexing for Genomic Intervals", pub.Title);
            Assert.Equal(2019, pub.Year);
            Assert.Equal("31", pub.Volume);
            Assert.Equal(10, pub.Number);
            Assert.Equal("2008-2021", pub.Pages);
            Assert.Equal("10.1109/TKDE.2018.2871031", pub.DOI);
            Assert.Equal(10, pub.Month);
            foreach (var keyword in keywords)
                Assert.Contains(pub.Keywords, k => k.Label == keyword);
        }

        [Theory]
        [InlineData(bibitemS00, BibTexEntryType.Article)]
        [InlineData(bibitemS01, BibTexEntryType.Article)]
        [InlineData(bibitemS02, BibTexEntryType.Misc)]
        [InlineData(bibitemS04, BibTexEntryType.Manual)]
        [InlineData(bibitemS05, BibTexEntryType.Phdthesis)]
        [InlineData(bibitemS06, BibTexEntryType.Book)]
        [InlineData(bibitemS07, BibTexEntryType.Techreport)]
        [InlineData(bibitemS08, BibTexEntryType.Unknown)]
        [InlineData(bibitemS09, BibTexEntryType.Book)]
        public void ExtractType(string bibitem, BibTexEntryType expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Type);
        }

        [Theory]
        [InlineData(bibitemS00, "A: title; on, some---topic")]
        [InlineData(bibitemS01, "a_title")]
        [InlineData(bibitemS02, "and a title")]
        [InlineData(bibitemS04, "my: title")]
        [InlineData(bibitemS05, "my thesis title")]
        [InlineData(bibitemS06, "MSPC, CPSM")]
        [InlineData(bibitemS07, "I am a tec rep.")]
        [InlineData(bibitemS08, "aaa: bbb (ccc)")]
        [InlineData(bibitemS09, "sometitle")]
        public void ExtractTitle(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Title);
        }

        [Theory]
        [InlineData(bibitemS00, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS01, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS02, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS03, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS04, null)]
        [InlineData(bibitemS05, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS06, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS07, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS10, "10.1109/TKDE.2018.2871031")]
        public void ExtractDOI(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.DOI);
        }

        [Theory]
        [InlineData(bibitemS00, "lName fName")]
        [InlineData(bibitemS01, "lname1 fname1", "lname{{\\\"e}} fname2", "lname3 fname{\\\"3}")]
        public void ExtractAuthors(string bibitem, string auth1, string auth2 = null, string auth3 = null)
        {
            // Arrange
            static string[] splitName(string name)
            {
                return new string[] { name.Split(' ')[1], name.Split(' ')[0] };
            }

            var names = new List<string[]> { splitName(auth1) };
            if (auth2 != null) names.Add(splitName(auth2));
            if (auth3 != null) names.Add(splitName(auth3));

            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            foreach (var name in names)
                Assert.Contains(pub.Authors, a => a.FirstName == name[0] && a.LastName == name[1]);
        }

        [Theory]
        [InlineData(bibitemS00, 2019)]
        [InlineData(bibitemS01, null)]
        [InlineData(bibitemS02, 2020)]
        public void ExtractYear(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Year);
        }

        [Theory]
        [InlineData(bibitemS00, null)]
        [InlineData(bibitemS06, 9)]
        [InlineData(bibitemS07, 12)]
        [InlineData(bibitemS08, 8)]
        public void ExtractMonth(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Month);
        }

        [Theory]
        [InlineData(bibitemS00, null)]
        [InlineData(bibitemS08, 1)]
        public void ExtractDay(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Day);
        }

        [Theory]
        [InlineData(bibitemS00, "journaltitle")]
        [InlineData(bibitemS01, "journal name")]
        [InlineData(bibitemS02, null)]
        public void ExtractJournal(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Journal);
        }

        [Theory]
        [InlineData(bibitemS01, "10")]
        [InlineData(bibitemS02, null)]
        [InlineData(bibitemS04, "11-A")]
        public void ExtractVolume(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Volume);
        }

        [Theory]
        [InlineData(bibitemS00, 21)]
        [InlineData(bibitemS01, 5)]
        [InlineData(bibitemS02, null)]
        public void ExtractNumber(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Number);
        }

        [Theory]
        [InlineData(bibitemS00, "a chapter in the journal")]
        [InlineData(bibitemS06, "chapter in the book")]
        [InlineData(bibitemS01, null)]
        public void ExtractChapter(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Chapter);
        }

        [Theory]
        [InlineData(bibitemS00, null)]
        [InlineData(bibitemS01, "123-456")]
        [InlineData(bibitemS02, "100--101")]
        [InlineData(bibitemS04, "A001-6")]
        public void ExtractPages(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Pages);
        }

        [Theory]
        [InlineData(bibitemS00, null)]
        [InlineData(bibitemS01, "publisher")]
        [InlineData(bibitemS06, "somepublisher")]
        public void ExtractPublisher(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Publisher);
        }

        [Theory]
        [InlineData(bibitemS00, "akeyword")]
        [InlineData(bibitemS01, null)]
        public void ExtractKeyword(string bibitem, string expValue)
        {
            // Arrange
            var keywords = new List<string> { expValue };

            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            if (expValue == null)
                Assert.Null(pub.Keywords);
            else
                foreach (var keyword in keywords)
                    Assert.Contains(pub.Keywords, k => k.Label == keyword);
        }

        [Theory]
        [InlineData(bibitemS02, ',', "firstkeyword, secondkeyword")]
        [InlineData(bibitemS04, ';', "firstkeyword; secondkeyword")]
        public void AssertAppreciationofKeywordDelimiter(string bibitem, char delimiter, string expValue)
        {
            // Arrange
            var expKeywords = new List<string>();
            var words = expValue.Split(delimiter);
            foreach (var word in words)
                expKeywords.Add(word.Trim());

            // Act
            _parser.KeywordsDelimiter = delimiter;
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            foreach (var keyword in expKeywords)
                Assert.Contains(pub.Keywords, k => k.Label == keyword);
        }

        [Theory]
        [InlineData("ID, author = {lName fName}, title = {title")]
        public void FailParsingMalformedInput(string malformedInput)
        {
            // Act 
            var success = _parser.TryParse(malformedInput, out Publication pub);

            // Assert
            Assert.False(success);
        }
    }
}
