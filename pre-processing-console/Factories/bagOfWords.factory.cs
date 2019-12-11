using System.Collections.Generic;

namespace pre_processing_console.factories
{
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

        public class LocalBagOfWords
        {
            private List<KeyValuePair<int, int>> _bag = new List<KeyValuePair<int, int>>();
            private string _text;
            private BagOfWordsFactory _bagOfWordsFactory;

            /// <summary>
            /// Create a instance to resolve bag of words of one text;
            /// </summary>
            /// <param name="bagOfWordsFactory">The parent factory to make calcs with dictionary</param>
            /// <param name="text">The full text to analyse</param>
            public LocalBagOfWords(BagOfWordsFactory bagOfWordsFactory, string text)
            {
                this._text = text;
                this._bagOfWordsFactory = bagOfWordsFactory;
            }

            private string[] PrepareTextToBag()
            {
                return new TextFactory(this._text)
                    .IgnoreCase()
                    .IgnoreLines()
                    .IgnoreDiacritics()
                    .IgnoreNonLetters()
                    .IgnoreOneCharWords()
                    .IgnoreTwoCharWords()
                    .ExtractWords();
            }

            public void AddWordToBag(string word) 
            {
                var globalDictionaryWordIndex   = this._bagOfWordsFactory.AddWordToGlobalDictionary(word);
                var localBagWord                = this._bag.Find(x => x.Key == globalDictionaryWordIndex.Key);

                // Check if word in in local bag
                // This is the WORDS FREQUENCIES
                this._bag.RemoveAll(x => x.Key == globalDictionaryWordIndex.Key);
                this._bag.Add(new KeyValuePair<int, int>(globalDictionaryWordIndex.Key,localBagWord.Value + 1));
            }

            public List<KeyValuePair<int, int>> GetBag() 
            {
                var bag = this._bag;
                bag.Sort((a, b) => a.Key - b.Key);

                return bag;
            }

            public List<KeyValuePair<int, int>> CreateBag()
            {
                var words = this.PrepareTextToBag();

                foreach (var word in words)
                {
                    this.AddWordToBag(word);
                };

                return this.GetBag();
            }

        }

        public KeyValuePair<int, string> AddWordToGlobalDictionary(string word)
        {
            var globalDictionaryWordIndex   = this._globalWordsDictionary.Find(x => x.Value == word);
            var isInGlobalDictionary        = globalDictionaryWordIndex.Value != null;
            var wordKeyValue                = new KeyValuePair<int, string>(this._globalWordsDictionary.Count, word);

            if (!isInGlobalDictionary) 
            {
                this._globalWordsDictionary.Add(wordKeyValue);
                return wordKeyValue;
            }
            else 
            {
                return globalDictionaryWordIndex;
            }
        }

        public List<KeyValuePair<int, string>> GetGlobalWordsDictionary() => this._globalWordsDictionary;
    }
}