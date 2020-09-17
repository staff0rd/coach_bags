using System;
using Xunit;
using coach_bags_selenium;
using Shouldly;

namespace Test
{
    public class HashtagGeneratorTests
    {
        HashtagGenerator _generator = new HashtagGenerator();

        [Theory]
        [InlineData("Off-White", "#offwhite")]
        [InlineData("L'Afshar", "#lafshar")]
        [InlineData("P.A.R.O.S.H.", "#parosh")]
        [InlineData("Nº21", "#n21")]
        [InlineData("RED(V)", "#redv")]
        public void Should_strip_punctuation(string input, string expected)
        {
            _generator.Generate(input).ShouldBe(expected);
        }

        [Theory]
        [InlineData("K Jacques", "#kjacques")]
        [InlineData("Issey Miyake Cauliflower", "#isseymiyakecauliflower")]
        [InlineData("A.W.A.K.E. Mode", "#awakemode")]
        public void Should_strip_spaces(string input, string expected)
        {
            _generator.Generate(input).ShouldBe(expected);
        }

        [Theory]
        [InlineData("AQUAZZURA", "#aquazzura")]
        [InlineData("Missoni", "#missoni")]
        public void Should_transform_to_lowercase(string input, string expected)
        {
            _generator.Generate(input).ShouldBe(expected);
        }

        [Theory]
        [InlineData("Givenchy", "#givenchy")]
        [InlineData("Birkenstock", "#birkenstock")]
        public void Should_prefix_with_hash(string input, string expected)
        {
            _generator.Generate(input).ShouldBe(expected);
        }
    }
}
