import { DocumentModel, DocumentKeywordModel } from './document.model';

export class GroupLearningModel {
  documents: DocumentModel[];
  keywordsAverage: DocumentKeywordModel[];
  keywordsLimit: number;
  tolerance: number;
}
