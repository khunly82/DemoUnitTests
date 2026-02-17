using DemoUnitTests.API.Utils;
using Microsoft.Extensions.Primitives;

namespace DemoUnitTests.Tests.API.Utils
{
    public class StringUtilsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("        ", "")]
        public void ToTitle_withEmptyString(string value, string expected)
        {
            string result = StringUtils.ToTitle(value);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToTitle_withWitheSpaces()
        {
            string initial = "Ceci   est une     chaine de caractères   ";
            string expected = "Ceci Est Une Chaine De Caractères";

            string result = StringUtils.ToTitle(initial);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("ceci est une chaine de caractères")]
        [InlineData("CECI EST UNE CHAINE DE CARACTÈRES")]
        [InlineData("cECI eST uNE cHAINE dE cARACTÈRES")]
        public void ToTitle_withVariousCase(string value)
        {
            string expected = "Ceci Est Une Chaine De Caractères";

            string result = StringUtils.ToTitle(value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("ceci est une école", "Ceci Est Une École")]
        [InlineData("ceci est un !", "Ceci Est Un !")]
        [InlineData("ça va ?", "Ça Va ?")]
        [InlineData("1,2,3 soleil", "1,2,3 Soleil")]
        [InlineData("1,2,3 💩", "1,2,3 💩")]
        [InlineData("µ", "Μ")]
        [InlineData("j'ai faim", "J'ai Faim")]
        public void ToTitle_withSpecialChars(string value, string expected)
        {
            string result = StringUtils.ToTitle(value);
            Assert.Equal(expected, result);
        }
    }
}
