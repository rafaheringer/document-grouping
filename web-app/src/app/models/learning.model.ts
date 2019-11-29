import { DocumentModel } from './document.model';

export class GroupLearningModel {
  documents: DocumentModel[];
  keywordsAverage: { [key: number]: number };
  keywordsLimit: number;
  tolerance: number;
  get wordsCount(): number {
    let sum = 0;

    Object.keys(this.keywordsAverage).forEach(key => {
      sum += this.keywordsAverage[key];
    });

    return sum;
  }
}