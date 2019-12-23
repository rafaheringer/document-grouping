using System;
using System.Collections.Generic;

namespace pre_processing_console.models
{
    public class TermFrequenceGroupTrainingModel
    {
        public Guid Id { get; set; }
        public List<KeyValuePair<int, double>> averageWordsCount { get; set; }
        public int keywordsLimit { get; set; }
        public double tolerance { get; set; }

        public List<PreprocessedDocumentModel> documents { get; set; }
    }

    public class TermFrequenceGroupProcessResultModel
    {
        public bool isCompatible { get; set; }
        public double calculatedSkore { get; set;}
    }
}