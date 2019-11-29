import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { DocumentModel, GlobalKeywordsModel } from 'src/app/models/document.model';
import { DOCUMENTSMOCK } from 'src/app/mocks/documents.mock';
import { KEYWORDSMOCK } from 'src/app/mocks/words.mock';
import { AngularFireDatabase } from '@angular/fire/database';

@Injectable({
  providedIn: 'root'
})
export class DocumentsService {

  constructor(private firebase: AngularFireDatabase) {

  }

  getWordsIndex(): Promise<GlobalKeywordsModel[]> {
    // return of(KEYWORDSMOCK);

    return new Promise(async (resolve, reject) => {

      try {
        this.firebase.list('/').valueChanges().subscribe((allValues) => {
          let correctValue;

          allValues.forEach(item => {
            // tslint:disable-next-line: no-string-literal
            if (!item['fileId']) {
              correctValue = item;
            }
          });

          resolve(correctValue);
        });


      } catch (ex) {
        reject(ex);
      }

    });
    return ;

  }

  getLatestProcessedDocuments(limit: number = 100): Observable<DocumentModel[]> {
    // return of(DOCUMENTSMOCK);
    return this.firebase.list<DocumentModel>('/').valueChanges();
  }
}
