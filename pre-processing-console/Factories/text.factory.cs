using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pre_processing_console.factories
{
    public class TextFactory
    {
        private string _text;
        private string[] _stopWords;

        /// <summary>
        /// Create a instance to manipulate texts to prepare to any operation, like Machine Learning processes.
        /// Usage exemple: new TextFactory("Lorem ipsum").IgnoreCase().IgnoreNonLetters().ExtractWords();
        /// </summary>
        /// <param name="text">The text to prepare.</param>
        public TextFactory(string text)
        {
            this._text = text;
            this._stopWords = new string[]{"ltda", "mei", "me", "na", "no", "da", "do", "de", "para", "e", "o", "a", "ou"};
        }

        /// <summary>
        /// Create a instance to manipulate texts to prepare to any operation, like Machine Learning processes.
        /// Usage exemple: new TextFactory("Lorem ipsum", new string[]{"it", "that", "was"}).IgnoreCase().IgnoreNonLetters().ExtractWords();
        /// </summary>
        /// <param name="text">The text to prepare.</param>
        /// <param name="stopWords">Stop words to ignore (eg. prepositions)</param>
        public TextFactory(string text, string[] stopWords)
        {
            this._text = text;
            this._stopWords = stopWords;
        }

        public TextFactory IgnoreCase()
        {
            this._text = this._text.ToLower();
            return this;
        }

        public TextFactory IgnoreNonLetters()
        {
            this._text = new string(this._text.Where(c => (char.IsLetter(c) || char.IsWhiteSpace(c))).ToArray());
            return this;
        }

        public TextFactory IgnoreDiacritics()
        {
            var chars = this._text.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            this._text = new string(chars).Normalize(NormalizationForm.FormC);
            return this;
        }

        public TextFactory IgnoreLines()
        {
            this._text = string.Join(" ", this._text.Split().Where(x => !new string[] { @"\r", @"\t", @"\n" }.Contains(x)));
            return this;
        }

        public TextFactory IgnoreOneCharWords()
        {
            var words = this.ExtractWords();
            this._text = String.Join(" ", words.Where(x => x.Length > 1));
            return this;
        }

        public TextFactory IgnoreTwoCharWords()
        {
            var words = this.ExtractWords();
            this._text = String.Join(" ", words.Where(x => x.Length > 1));
            return this;
        }

        public string[] ExtractWords() => this._text.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        public string GetTransformedText() => this._text;
    }
}