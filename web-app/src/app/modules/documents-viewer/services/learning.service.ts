import { Injectable } from '@angular/core';
import { DocumentModel, GlobalKeywordsModel } from 'src/app/models/document.model';
import { GroupLearningModel } from 'src/app/models/learning.model';
import { AngularFireDatabase } from '@angular/fire/database';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LearningService {

  private _limitKeywords = 20;
  private _tolerance = 0.30;
  private _equalizeGroupWhenDocumentCountIs = 3;
  private _groups: GroupLearningModel[] = [];

  constructor(private firebase: AngularFireDatabase) { }

  private compareDocumentAgainstGroup(document: DocumentModel, group: GroupLearningModel): boolean {
    let finalSkore = 0;
    let skores = [];

    const wordsCount = group.keywordsAverage.reduce((a, b) => (a + b.count), 0);

    group.keywordsAverage.forEach(keyword => {
      const keyAverageCount = group.keywordsAverage.find(x => x.key === keyword.key).count;
      const weight = keyAverageCount / wordsCount;
      let skore = 0;

      if (typeof(document.keywordsCount.find(x => x.key === keyword.key)) !== 'undefined') {
        const keyDocumentCount = document.keywordsCount.find(x => x.key === keyword.key).count;
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
    documents.forEach((document, index) => {
      document.keywordsCount = document.keywordsCount.sort((a, b) => b.count - a.count).slice(0, this._limitKeywords);
      const groupLength = this._groups.length;

      // First group
      if (groupLength === 0) {
        let gp = new GroupLearningModel();
        gp.documents = [document];
        gp.keywordsAverage = document.keywordsCount;
        gp.keywordsLimit = this._limitKeywords;
        gp.tolerance = this._tolerance;
        this.addGroup(gp);
      } else {
        // Compare with all groups
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < groupLength; i++) {
          if (this.compareDocumentAgainstGroup(document, this._groups[i])) {
            this._groups[i].documents.push(document);
            break;
          } else if(i === groupLength - 1) {
            let gp = new GroupLearningModel();
            gp.documents = [document];
            gp.keywordsAverage = document.keywordsCount;
            gp.keywordsLimit = this._limitKeywords;
            gp.tolerance = this._tolerance;
            this.addGroup(gp);
          }
        }

      }
    });
  }

  private addGroup(group: GroupLearningModel) {
    this._groups.push(group);
  }

  public getGroups(): GroupLearningModel[] {
    return this._groups;
  }
}
