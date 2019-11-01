using Genometric.BibitemParser.UnitTests.Model;
using System.Collections.Generic;
using Xunit;

namespace Genometric.BibitemParser.UnitTests
{
    public class ExtractFields
    {
        private readonly Parser<Publication, Author> _parser;

        private const string bibitemS1 = "@article{ID, author = {lName fName}, title = {A: title; on, some---topic}, journal = {journaltitle}, year = {2019}, doi={10.1109/TKDE.2018.2871031}, number={21}, chapter={a chapter in the journal}}";
        private const string bibitemS2 = "@article{id, title={a_title}, author={lname1, fname1 and lname2, fname2 and lname3, fname3}, journal={journal name}, publisher={publisher}, volume = {10}, issue = {5}, pages = {123-456}}";
        private const string bibitemS3 = "@misc{this_is_my_id, author = {first_name1, m.lastname1 and firstname_2, lastname2}, title = {and a title }, year = {2020}, pages={100--101} }";
        private const string bibitemS4 = "@Manual{,title = {my: title},author = {A. lName and B. lName},year = {2019},note = {I am optional},url = {https://https://genometric.github.io/MSPC/}, pages={ A001-6}}";
        private const string bibitemS5 = "@PhdThesis{,title = {my thesis title},author = {V J and A B. C and firstName LastName},url = {https://github.com/Genometric/MSPC/},school = {polimi},year = {2016},}";
        private const string bibitemS6 = "@Book{,title = {MSPC, CPSM},year = {2019},author = {V B. J and otherFirstName C. otherLastName}, publisher = {somepublisher},month = {9}, chapter=  {chapter in the book}}";
        private const string bibitemS7 = "@TechReport{,author = {abc efg and hjk lmn},title = {I am a tec rep.},institution = {xyz, Dep. rnd},year = {2020},type = {Technical report}, month = {December},}";
        private const string bibitemS8 = "@cannotguess{,author = {hmmm haaa},title = {aaa: bbb (ccc)},year = {2019}, month = {Aug},}";

        // This sample has line breaks (i.e., the '\r' and '\n' characters) 
        // and tabs (i.e., the '\t' character); hence can assert if parser can 
        // still parse data while such characters are present. 
        private const string bibitemS9 =
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
            _parser = new Parser<Publication, Author>(
                new PublicationConstructor(),
                new AuthorConstructor());
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
            Assert.Equal(31, pub.Volume);
            Assert.Equal(10, pub.Number);
            Assert.Equal("2008-2021", pub.Pages);
            Assert.Equal("10.1109/TKDE.2018.2871031", pub.DOI);
            Assert.Equal(10, pub.Month);
        }

        [Theory]
        [InlineData(bibitemS1, BibTexEntryType.Article)]
        [InlineData(bibitemS2, BibTexEntryType.Article)]
        [InlineData(bibitemS3, BibTexEntryType.Misc)]
        [InlineData(bibitemS4, BibTexEntryType.Manual)]
        [InlineData(bibitemS5, BibTexEntryType.Phdthesis)]
        [InlineData(bibitemS6, BibTexEntryType.Book)]
        [InlineData(bibitemS7, BibTexEntryType.Techreport)]
        [InlineData(bibitemS8, BibTexEntryType.Unknown)]
        [InlineData(bibitemS9, BibTexEntryType.Book)]
        public void ExtractType(string bibitem, BibTexEntryType expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Type);
        }

        [Theory]
        [InlineData(bibitemS1, "A: title; on, some---topic")]
        [InlineData(bibitemS2, "a_title")]
        [InlineData(bibitemS3, "and a title")]
        [InlineData(bibitemS4, "my: title")]
        [InlineData(bibitemS5, "my thesis title")]
        [InlineData(bibitemS6, "MSPC, CPSM")]
        [InlineData(bibitemS7, "I am a tec rep.")]
        [InlineData(bibitemS8, "aaa: bbb (ccc)")]
        [InlineData(bibitemS9, "sometitle")]
        public void ExtractTitle(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Title);
        }

        [Theory]
        [InlineData(bibitemS1, "10.1109/TKDE.2018.2871031")]
        [InlineData(bibitemS2, null)]
        public void ExtractDOI(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.DOI);
        }

        [Theory]
        [InlineData(bibitemS1, "lName fName")]
        [InlineData(bibitemS2, "lname1 fname1", "lname2 fname2", "lname3 fname3")]
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
        [InlineData(bibitemS1, 2019)]
        [InlineData(bibitemS2, null)]
        [InlineData(bibitemS3, 2020)]
        public void ExtractYear(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Year);
        }

        [Theory]
        [InlineData(bibitemS1, null)]
        [InlineData(bibitemS6, 9)]
        [InlineData(bibitemS7, 12)]
        [InlineData(bibitemS8, 8)]
        public void ExtractMonth(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Month);
        }

        [Theory]
        [InlineData(bibitemS1, "journaltitle")]
        [InlineData(bibitemS2, "journal name")]
        [InlineData(bibitemS3, null)]
        public void ExtractJournal(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Journal);
        }

        [Theory]
        [InlineData(bibitemS2, 10)]
        [InlineData(bibitemS3, null)]
        public void ExtractVolume(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Volume);
        }

        [Theory]
        [InlineData(bibitemS1, 21)]
        [InlineData(bibitemS2, 5)]
        [InlineData(bibitemS3, null)]
        public void ExtractNumber(string bibitem, int? expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Number);
        }

        [Theory]
        [InlineData(bibitemS1, "a chapter in the journal")]
        [InlineData(bibitemS6, "chapter in the book")]
        [InlineData(bibitemS2, null)]
        public void ExtractChapter(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Chapter);
        }

        [Theory]
        [InlineData(bibitemS1, null)]
        [InlineData(bibitemS2, "123-456")]
        [InlineData(bibitemS3, "100--101")]
        [InlineData(bibitemS4, "A001-6")]
        public void ExtractPages(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Pages);
        }

        [Theory]
        [InlineData(bibitemS1, null)]
        [InlineData(bibitemS2, "publisher")]
        [InlineData(bibitemS6, "somepublisher")]
        public void ExtractPublisher(string bibitem, string expValue)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expValue, pub.Publisher);
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
