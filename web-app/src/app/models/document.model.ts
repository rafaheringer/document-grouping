export class DocumentKeywordModel {
  key: number;
  count: number;
}

export class DocumentModel {
  fileName: string;
  fullText: string;
  skore?: number;
  keywordsCount: DocumentKeywordModel[];
}

export class GlobalKeywordsModel {
  Key: number;
  Value: string;
}
