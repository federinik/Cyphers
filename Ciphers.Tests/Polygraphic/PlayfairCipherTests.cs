using Ciphers.SubstitutionCiphers.Keyword;
using Shouldly;
using Xunit;

namespace Ciphers.Polygraphic
{
    public class PlayfairCipherTests
    {
        private readonly IKeywordCipher _cipher = new PlayfairCipher();

        /// <summary>
        /// Verifies that
        /// - Duplicate letters are removed
        /// - Order is preserved
        /// - Keyword is stored in lower case characters
        /// </summary>
        [Fact]
        public void VerifyUniqueKeywordLetters()
        {
            _cipher.SetKeyword("KEY WORD Q");
            _cipher.GetKeyword().ShouldBe("keyword");
        }

        [Fact]
        public void VerifyEncodeDecode()
        {
            // Arrange
            var message = "thisisasecretmessage";
            _cipher.SetKeyword("keyword");

            // Act
            var encoded = _cipher.Encode(message);
            var decoded = _cipher.Decode(encoded);

            // Asset
            encoded.ShouldNotBe(decoded);
            encoded.ShouldBe("vfjpjpcnoddkulomncmd");
            decoded.ShouldBe(message);
        }

        [Fact]
        public void VerifyEncodeDecodeOddText()
        {
            // Arrange
            var message = "othisisasecretmessage";
            _cipher.SetKeyword("keyword");

            // Act
            var encoded = _cipher.Encode(message);
            var decoded = _cipher.Decode(encoded);

            // Asset
            encoded.ShouldNotBe(decoded);
            encoded.ShouldBe("kzijpjncmordkuudpzncmd");
            decoded.ShouldBe("othisisasecretmesxsage");
        }
    }
}
