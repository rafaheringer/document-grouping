using System;
using System.Collections.Generic;

namespace pre_processing_console.models
{
    public class JaccardGroupTrainingModel
    {
        public Guid Id { get; set; }
        public List<KeyValuePair<int, int>> averageWordsCount { get; set; }
        public int keywordsLimit { get; set; }
        public double tolerance { get; set; }

        public List<PreprocessedDocumentModel> documents { get; set; }
    }

    public class JaccardGroupProcessResultModel
    {
        public bool isCompatible { get; set; }
        public double calculatedSkore { get; set;}
    }
}