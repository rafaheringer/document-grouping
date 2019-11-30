using System.Collections.Generic;

namespace pre_processing_console.factories 
{
    // Reference: https://nickgrattan.wordpress.com/2014/06/09/bag-of-words-and-frequency-distributions-in-c/
    public class BagOfWordsFactory 
    {
        private List<KeyValuePair<int, string>> _globalWordsDictionary;
        
        /// <summary>
        /// Functions to help construct a Bag of Words model. You need to pass one KeyValuePair of words dictionary. 
        /// All new words are storage inside the class and you can persist anywhere.
        /// </summary>
        /// <param name="initialWordsDictionary"></param>
        public BagOfWordsFactory(List<KeyValuePair<int, string>> initialWordsDictionary) 
        {
            this._globalWordsDictionary = initialWordsDictionary;
        }

        /// <summary>
        /// Functions to help construct a Bag of Words model with blank words dictionary.
        /// All new words are storage inside the class and you can persist anywhere.
        /// </summary>
        public BagOfWordsFactory() 
        {
            this._globalWordsDictionary = new List<KeyValuePair<int, string>>();
        }

        public string[] PrepareTextToBag(string text) {
            return new TextFactory(text)
                .IgnoreCase()
                .IgnoreLines()
                .IgnoreDiacritics()
                .IgnoreNonLetters()
                .IgnoreOneCharWords()
                .IgnoreTwoCharWords()
                .ExtractWords();
        }

        public List<KeyValuePair<int, string>> GetGlobalWordsDictionary() => this._globalWordsDictionary;
    }
}