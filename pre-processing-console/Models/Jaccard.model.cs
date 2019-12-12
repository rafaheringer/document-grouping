using System.Collections.Generic;

namespace pre_processing_console.models
{
    public class JaccardGroupModel
    {
        public List<KeyValuePair<int, int>> averageWordsCount { get; set; }
        public int keywordsLimit { get; set; }
        public double tolerance { get; set; }
    }
}