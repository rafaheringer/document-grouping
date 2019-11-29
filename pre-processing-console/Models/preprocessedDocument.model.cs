using System.Collections.Generic;

public class PreprocessedDocumentModel {
    public string fileName {get; set;}
    public string fileId {get; set;}
    public string fullText {get; set;}
    public List<Dictionary<int, int>> keywordsCount {get; set;}
}