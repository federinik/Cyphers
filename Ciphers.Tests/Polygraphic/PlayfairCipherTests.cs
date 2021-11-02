using System;
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
        /// - "Q" character is ignored
        /// </summary>
        [Fact]
        public void VerifyUniqueKeywordLetters()
        {
            // Arrange
            _cipher.SetKeyword("KEY WORD QK");

            // Assert
            _cipher.GetKeyword().ShouldBe("keyword");
        }

        [Fact]
        public void VerifyOddTextNotAllowed()
        {
            // Arrange
            var cipher = "this test has an odd length"; // 27 characters
            _cipher.SetKeyword("keyword");

            // Assert
            Should.Throw(() => _cipher.Decode(cipher), typeof(ArgumentException));
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

            // Assert
            encoded.ShouldNotBe(decoded);
            encoded.ShouldBe("vfjpjpcnoddkulomncmd");
            decoded.ShouldBe(message);
        }

        [Fact]
        public void VerifyEncodeDecodeOddText()
        {
            // Arrange
            var message = "othisisasecretmessage"; // 21 characters
            _cipher.SetKeyword("keyword");

            // Act
            var encoded = _cipher.Encode(message);
            var decoded = _cipher.Decode(encoded);

            // Assert
            encoded.ShouldNotBe(decoded);
            encoded.ShouldBe("kzijpjncmordkuudpzncmd");
            decoded.ShouldBe("othisisasecretmesxsage");
        }
    }
}
