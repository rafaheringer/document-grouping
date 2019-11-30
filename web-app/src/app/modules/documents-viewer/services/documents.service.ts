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

  getWordsIndex(): Observable<GlobalKeywordsModel[]> {
    // return of(KEYWORDSMOCK);
    return this.firebase.list<GlobalKeywordsModel>('/global-words').valueChanges();
  }

  getLatestProcessedDocuments(limit: number = 100): Observable<DocumentModel[]> {
    // return of(DOCUMENTSMOCK);
    return this.firebase.list<DocumentModel>('/preprocessed-documents').valueChanges();
  }
}