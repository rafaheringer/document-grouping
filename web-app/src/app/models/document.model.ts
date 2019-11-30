export class DocumentKeywordModel {
  key: number;
  count: number;
}

export class DocumentModel {
  fileName: string;
  fullText: string;
  skore?: number;
  keywordsCount: DocumentKeywordModel[];
  get wordsCount(): number {
    let sum = 0;

    Object.keys(this.keywordsCount).forEach(key => {
      sum += this.keywordsCount[key];
    });

    return sum;
  }
}

export class GlobalKeywordsModel {
  Key: number;
  Value: string;
}
