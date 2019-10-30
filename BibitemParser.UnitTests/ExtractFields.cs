using Genometric.BibitemParser;
using Genometric.BibitemParser.UnitTests.Model;
using Xunit;

namespace Genometric.BibitemParser.UnitTests
{
    public class ExtractFields
    {
        private readonly Parser<Publication, Author> _parser;

        private const string bibitemSample1 = "@article{id, title={a_title}, author={lname1, fname1, lname2, fname2 and lname3, fname3}, journal={journal name}, year={2020}, publisher={publisher}}";

        public ExtractFields()
        {
            _parser = new Parser<Publication, Author>(
                new PublicationConstructor(), 
                new AuthorConstructor());
        }

        [Theory]
        [InlineData(bibitemSample1, BibTexEntryType.Article)]
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
