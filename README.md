# Bibitem Parser

[![NuGet Badge](https://buildstats.info/nuget/Genometric.BibitemParser?vWidth=50&dWidth=50)](https://www.nuget.org/packages/Genometric.BibitemParser)

The Bibitem parser takes a LaTeX `bibitem` as input and parses it to a 
[`Publication`](https://github.com/Genometric/BibitemParser/blob/f5e50f9344fa38b64e78c36cfc62b5d84b29ece9/BibitemParser/Model/Publication.cs#L6) 
object. The parser has the following characteristics: 

- It parses the given information without any required fields. 
  For instance, it reads the title of book in the following `bibitem`
  while it is missing other information such as _author name_ and _publisher_. 
  ```
  @"@book{, title = {Awesome Book}}"
  ```
  
- The parser reads common LaTeX typeset used in `bibitem` such as 
  accented letters (e.g., `\"e`).
  
- Accounts for some common inconsistencies in `bibitem`. 
  For instance `volume = {{10}}` while it should be `volume = {10}`, 
  or `doi={NA}` while instead this field should not be given.
  
- It comes with generic and concrete constructors, hence can use 
  built-in models or use user-provided types. For instance:
  ```csharp
  // Concrete
  var parser = new Parser();
  
  // Generic
  var parser = new Parser<Publication, Author, Keyword>(
    new PublicationConstructor(), 
    new AuthorConstructor(), 
    new KeywordConstructor());
  
  // For details, see examples below.
  ```
  
- Accessible and modifiable 
  [regex pattern](https://github.com/Genometric/BibitemParser/blob/f5e50f9344fa38b64e78c36cfc62b5d84b29ece9/BibitemParser/Parser.cs#L113),
  [custom attribute delimiter](https://github.com/Genometric/BibitemParser/blob/f5e50f9344fa38b64e78c36cfc62b5d84b29ece9/BibitemParser/Parser.cs#L87), 
  [custom keyword delimiter](https://github.com/Genometric/BibitemParser/blob/f5e50f9344fa38b64e78c36cfc62b5d84b29ece9/BibitemParser/Parser.cs#L119), and 
  [stop-words](https://github.com/Genometric/BibitemParser/blob/f5e50f9344fa38b64e78c36cfc62b5d84b29ece9/BibitemParser/Parser.cs#L59).
  
# Examples

## Example 1: Use concrete constructor

This is the simplest usage of the parser where it parses 
`bibitem` to built-in models.

```csharp
using Genometric.BibitemParser;

// Sample input.
var bibitem = @"@ARTICLE{8468044, author={Jalili, Vahid}, title={Next Generation Indexing for Genomic Intervals}, year={2019}, doi={10.1109/TKDE.2018.2871031}}";

// Initialize the parser.
var parser = new Parser();

// Parse the sample bibitem.
bool status = parser.TryParse(bibitem, out Publication pub);

// Assert the parsed information.
Xunit.Assert.True(status);
Xunit.Assert.Contains(pub.Authors, a => a.FirstName == "Vahid" && a.LastName == "Jalili");
Xunit.Assert.Equal(pub.DOI, "10.1109/TKDE.2018.2871031");
```

## Example 2: Parse into custom types

This example shows how to provide custom types to the parser.
The provided types implement their respective interfaces, which 
are defined in 
[this directory](https://github.com/Genometric/BibitemParser/tree/master/BibitemParser/Interfaces).
See built-in models as example on how to implement custom types
(e.g., the built-in 
[`Publication`](https://github.com/Genometric/BibitemParser/blob/f5e50f9344fa38b64e78c36cfc62b5d84b29ece9/BibitemParser/Model/Publication.cs#L6) 
type).

```csharp
using Genometric.BibitemParser;
using Genometric.BibitemParser.Model;
using Genometric.BibitemParser.Constructors;

// Sample input.
var bibitem = @"@ARTICLE{8468044, author={Jalili, Vahid}, title={Next Generation Indexing for Genomic Intervals}, year={2019}, doi={10.1109/TKDE.2018.2871031}}";

// Initialize the parser.
var parser = new Parser<Publication, Author, Keyword>(
    new PublicationConstructor(), 
    new AuthorConstructor(), 
    new KeywordConstructor());

// Parse the sample bibitem.
bool status = parser.TryParse(bibitem, out Publication pub);

// Assert the parsed information.
Xunit.Assert.True(status);
Xunit.Assert.Contains(pub.Authors, a => a.FirstName == "Vahid" && a.LastName == "Jalili");
Xunit.Assert.Equal(pub.DOI, "10.1109/TKDE.2018.2871031");
```
