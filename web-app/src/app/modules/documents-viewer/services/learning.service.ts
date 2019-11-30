import { Injectable } from '@angular/core';
import { DocumentModel, GlobalKeywordsModel } from 'src/app/models/document.model';
import { GroupLearningModel } from 'src/app/models/learning.model';
import { AngularFireDatabase } from '@angular/fire/database';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LearningService {

  private _limitKeywords = 5;
  private _tolerance = 0.20;
  private _equalizeGroupWhenDocumentCountIs = 3;
  private _groups: GroupLearningModel[] = [];

  constructor(private firebase: AngularFireDatabase) { }

  private compareDocumentAgainstGroup(document: DocumentModel, group: GroupLearningModel): boolean {
    let finalSkore = 0;
    let skores = [];

    const wordsCount = group.wordsCount;

    group.keywordsAverage.forEach(keyword => {
      const keyAverageCount = group.keywordsAverage.find(x => x.key === keyword.key).count;
      const keyDocumentCount = document.keywordsCount.find(x => x.key === keyword.key).count;
      const weight = keyAverageCount / wordsCount;
      let skore = 0;

      if (keyDocumentCount) {
        const max = keyAverageCount + (keyAverageCount * group.tolerance);
        const min = keyAverageCount - (keyAverageCount * group.tolerance);

        // Caraca sou muito ruim de montar fórmula matemática
        if (keyDocumentCount >= keyAverageCount) {
          skore = (100 + (this._tolerance * 10) - ((keyDocumentCount * 100) / (max))) / 10;
          if (skore < 0) {
            skore = 0;
          }
        } else {
          skore = Math.abs((100 + (this._tolerance * 10) - ((keyDocumentCount * 100) / (min))) / 10);
          if (skore < 0) {
            skore = 0;
          }
        }
      }

      skore = Math.round(skore * 100) / 100;

      skores.push({weight, skore});
    });

    skores.forEach(skore => {
      finalSkore += skore.weight * skore.skore;
    });

    document.skore = Math.round(finalSkore * 100) / 100;

    return finalSkore >= 1 - this._tolerance;
  }

  private equalizeGroup(group: GroupLearningModel) {

  }

  public compareOneAgainstAll(documents: DocumentModel[]) {
    let groupLength = this._groups.length;

    documents.forEach((document, index) => {
      document.keywordsCount = document.keywordsCount.sort((a, b) => b.count - a.count);
      // First group
      if (index === 0) {
        let gp = new GroupLearningModel();
        gp.documents = [document];
        gp.keywordsAverage = document.keywordsCount;
        gp.keywordsLimit = this._limitKeywords;
        gp.tolerance = this._tolerance;
        this._groups.push(gp);
        groupLength++;
      } else {
        // Compare with all groups
        // tslint:disable-next-line: prefer-for-of

        for (let i = 0; i < groupLength; i++) {
          groupLength = this._groups.length;

          if (this.compareDocumentAgainstGroup(document, this._groups[i])) {
            this._groups[i].documents.push(document);
            break;
          }

          // Create new group
          else if(i === groupLength - 1) {
            let gp = new GroupLearningModel();
            gp.documents = [document];
            gp.keywordsAverage = document.keywordsCount;
            gp.keywordsLimit = this._limitKeywords;
            gp.tolerance = this._tolerance;
            this._groups.push(gp);
          }
        }

      }
    });
  }

  public getGroups(): GroupLearningModel[] {
    return this._groups;
  }
}
