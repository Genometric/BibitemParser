using Genometric.BibitemParser.UnitTests.Model;
using Xunit;

namespace Genometric.BibitemParser.UnitTests
{
    public class ExtractFields
    {
        private readonly Parser<Publication, Author> _parser;

        private const string bibitemS1 = "@article{ID, author = {fName lName}, title = {A: title; on, some---topic}, journaltitle = {journaltitle}, date = {date}, }";
        private const string bibitemS2 = "@article{id, title={a_title}, author={lname1, fname1 and lname2, fname2 and lname3, fname3}, journal={journal name}, year={2020}, publisher={publisher}}";
        private const string bibitemS3 = "@misc{this_is_my_id, author = {first_name1, m.lastname1 and firstname_2, lastname2}, title = {and a title }, date = {Oct 30, 2019}, }";
        private const string bibitemS4 = "@Manual{,title = {my: title},author = {A. lName and B. lName},year = {2019},note = {I am optional},url = {https://https://genometric.github.io/MSPC/},}";
        private const string bibitemS5 = "@PhdThesis{,title = {my thesis title},author = {V J and A B. C and firstName LastName},url = {https://github.com/Genometric/MSPC/},school = {polimi},year = {2016},}";
        private const string bibitemS6 = "@Book{,title = {MSPC, CPSM},year = {2019},author = {V B. J and otherFirstName C. otherLastName}, publisher = {somepublisher},}";
        private const string bibitemS7 = "@TechReport{,author = {abc efg and hjk lmn},title = {I am tec rep.},institution = {xyz, Dep. rnd},year = {2020},type = {Technical report},}";
        private const string bibitemS8 = "@cannotguess{,author = {hmmm haaa},title = {aaa: bbb (ccc)},year = {2019},}";

        // This sample has line breaks (i.e., the '\r' and '\n' characters) 
        // and tabs (i.e., the '\t' character); hence can assert if parser can 
        // still parse data while such characters are present. 
        private const string bibitemS9 =
            @"@book{ID,
	author = {fName M. lName},
	title = {sometitle},
	date = {2019},
}";

        public ExtractFields()
        {
            _parser = new Parser<Publication, Author>(
                new PublicationConstructor(), 
                new AuthorConstructor());
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
        public void ExtractType(string bibitem, BibTexEntryType expectedType)
        {
            // Act
            var success = _parser.TryParse(bibitem, out Publication pub);

            // Assert
            Assert.True(success);
            Assert.Equal(expectedType, pub.Type);
        }
    }
}
