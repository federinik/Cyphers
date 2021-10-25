using System;
using System.Linq;
using Ciphers.SubstitutionCiphers.Keyword;

namespace Ciphers.Polygraphic
{
    public class PlayfairCipher : IKeywordCipher
    {
        private const int ROWS = 5;
        private const int COLS = 5;
        private const string ALPHABET = "abcdefghijklmnoprstuvwxyz"; // omit q to have 5x5 = 25 letters
        private const string PAD_CHAR = "x";

        private readonly char[,] _table = new char[ROWS, COLS];
        private string _keyword;

        public PlayfairCipher() { }

        public PlayfairCipher(string keyword) =>
            SetKeyword(keyword);

        public string Decode(string cipherText)
        {
            if (cipherText.Length % 2 != 0)
                throw new ArgumentException("Text length must be even in order to be a result of a Playfair encryption", nameof(cipherText));

            return Playfair(cipherText, false);
        }

        public string Encode(string plainText)
        {
            var toEncode = plainText.Replace(PAD_CHAR, string.Empty);

            for (var i = 0; i < toEncode.Length - 1; i += 2)
                if (toEncode[i] == toEncode[i + 1])
                    toEncode = toEncode.Insert(++i, PAD_CHAR);

            toEncode = toEncode.Length % 2 == 0 ? toEncode : $"{toEncode}{PAD_CHAR}";

            return Playfair(toEncode, true);
        }

        private string Playfair(string text, bool isEncryption)
        {
            var toReturn = string.Empty;
            var shift = isEncryption ? 1 : -1;

            static int wrap(int dividend, int divisor)
            {
                var mod = dividend % divisor;

                return mod >= 0 ? mod : divisor + mod;
            }

            for (var i = 0; i < text.Length - 1; i++)
            {
                char c1 = text[i], c2 = text[++i], d1, d2;
                var (row1, col1) = FindInTable(c1);
                var (row2, col2) = FindInTable(c2);

                if (row1 == row2)
                {
                    //If the letters appear on the same row of your table, replace them with the letters to their
                    //immediate right respectively, wrapping around to the left side of the row if necessary.
                    d1 = _table[row1, wrap(col1 + shift, COLS)];
                    d2 = _table[row2, wrap(col2 + shift, COLS)];
                }
                else if (col1 == col2)
                {
                    //If the letters appear on the same column of your table, replace them with
                    //the letters immediately below, wrapping around to the top if necessary.
                    d1 = _table[wrap(row1 + shift, ROWS), col1];
                    d2 = _table[wrap(row2 + shift, ROWS), col2];
                }
                else
                {
                    //If the letters are on different rows and columns, replace them with the letters on the same row
                    //respectively but at the other pair of corners of the rectangle defined by the original pair.
                    //The order is important - the first letter of the pair should be replaced first. 
                    d1 = _table[row1, col2];
                    d2 = _table[row2, col1];
                }

                toReturn += char.IsUpper(c1) ? char.ToUpper(d1) : d1;
                toReturn += char.IsUpper(c2) ? char.ToUpper(d2) : d2;
            }

            return toReturn;
        }

        public string GetKeyword() =>
            _keyword;

        public void SetKeyword(string keyword)
        {
            _keyword = string.Empty;

            int i = 0, j = 0;

            void appendCharacter(char c)
            {
                _table[i, j++] = c;

                if (j >= ROWS)
                {
                    j = 0;
                    i++;
                }
            }

            foreach (var kw in keyword.Distinct())
            {
                var c = char.ToLower(kw);
                if (!char.IsLower(c) || c == 'q')
                    continue;

                _keyword += c;

                appendCharacter(c);
            }

            foreach (var c in ALPHABET.Except(_keyword).ToArray())
                appendCharacter(c);
        }

        private (int row, int col) FindInTable(char c)
        {
            var lowC = char.ToLower(c);

            for (var i = 0; i < ROWS; i++)
                for (var j = 0; j < COLS; j++)
                    if (_table[i, j] == lowC)
                        return (i, j);

            throw new Exception($"Character '{c}' is not a character suitable for encryption"); // should never occur
        }
    }
}
