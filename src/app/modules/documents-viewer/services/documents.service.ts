import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { DocumentModel } from 'src/app/models/document.model';
import { DOCUMENTSMOCK } from 'src/app/mocks/documents.mock';
import { KEYWORDSMOCK } from 'src/app/mocks/words.mock';

@Injectable({
  providedIn: 'root'
})
export class DocumentsService {

  constructor() {

  }

  getWordsIndex(): Observable<{ [key: number]: number }> {
    return of(KEYWORDSMOCK);
  }

  getLatestProcessedDocuments(limit: number = 100): Observable<DocumentModel[]> {
    return of(DOCUMENTSMOCK);
  }
}
