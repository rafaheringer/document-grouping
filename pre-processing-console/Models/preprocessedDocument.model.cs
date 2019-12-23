using System;
using System.Collections.Generic;

namespace pre_processing_console.models
{
    public class PreprocessedDocumentModel
    {
        public string fileName { get; set; }
        public string fileId { get; set; }
        public string fullText { get; set; }
        public List<KeyValuePair<int, int>> keywordsCount { get; set; }
    }
}