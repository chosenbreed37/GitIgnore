namespace GitIgnore.Tests
{
    using lib;
    using NUnit.Framework;

    public class PatternParserTests
    {
        private PatternParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new PatternParser();
        }

        [Test]
        public void PatternParser_Parse_WithSpecificFilePattern_ShouldMatchMatchingPaths()
        {
            // Arrange
            var pattern = "*.png";

            // Act
            Pattern result = _parser.Parse(pattern);

            // Assert
            Assert.That(result.IsInclusive, Is.False);
            Assert.That(result.Expression.ToString(), Is.EqualTo("(?:.+/)?.*\\.png"));
            Assert.That(result.Expression.IsMatch("clap.png"));
            Assert.That(result.Expression.IsMatch("random/clap.png"));
            Assert.That(result.Expression.IsMatch("random/crap/clap.png"));
        }

        [Test]
        public void PatternParser_Parse_WithDirectoryName_ShouldMatchMatchingPaths()
        {
            // Arrange
            var pattern = "random/";

            // Act
            Pattern result = _parser.Parse(pattern);

            // Assert
            Assert.That(result.IsInclusive, Is.False);
            Assert.That(result.Expression.ToString(), Is.EqualTo("(?:.+/)?random/.+"));
            Assert.That(result.Expression.IsMatch("pop/random/ok.txt"));
            Assert.That(result.Expression.IsMatch("crap/pouf/random/other/rand.c"));
        }

        [Test]
        public void PatternParser_Parse_WithDirectoryNameRelativeToRoot_ShouldMatchMatchingPaths()
        {
            // Arrange 
            var pattern = "!/extra/drop/";

            // Act
            Pattern result = _parser.Parse(pattern);

            // Assert
            Assert.That(result.IsInclusive, Is.True);
            Assert.That(result.Expression.ToString(), Is.EqualTo("extra/drop/.+"));
            Assert.That(result.Expression.IsMatch("/extra/drop/random.txt"));
        }
    }
}
