using System.Collections.Generic;

public class PreprocessedDocumentModel {
    public string fileName {get; set;}
    public string fileId {get; set;}
    public string fullText {get; set;}
    public List<KeyValuePair<int, int>> keywordsCount {get; set;}
}

public class KeyCountModel {
    public int key {get; set;}
    public int count {get;set;}
}