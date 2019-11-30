using System.Collections.Generic;

namespace pre_processing_console.factories 
{
    // Reference: https://nickgrattan.wordpress.com/2014/06/09/bag-of-words-and-frequency-distributions-in-c/
    public class BagOfWordsFactory 
    {
        public List<KeyValuePair<int, string>> globalWordsDictionary { get; }
        
        public BagOfWordsFactory() 
        {

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
    }
}