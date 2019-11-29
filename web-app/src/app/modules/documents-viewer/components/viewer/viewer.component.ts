import { Component, OnInit } from '@angular/core';
import { DocumentModel, GlobalKeywordsModel } from 'src/app/models/document.model';
import { DocumentsService } from '../../services/documents.service';
import { LearningService } from '../../services/learning.service';
import { GroupLearningModel } from 'src/app/models/learning.model';

@Component({
  selector: 'app-viewer',
  templateUrl: './viewer.component.html',
  styleUrls: ['./viewer.component.sass']
})
export class ViewerComponent implements OnInit {

  public groups: GroupLearningModel[] = [];
  public wordsIndex: GlobalKeywordsModel[] = [];

  private _documents: DocumentModel[] = [];

  public getWordByIndex(index: string) {
    // tslint:disable-next-line: radix
    let i = parseInt(index);
    return this.wordsIndex.find(a => a.Key === i).Value;
  }

  constructor(
    private documentsService: DocumentsService,
    private learningService: LearningService
    ) { }

  private updateWordsIndex(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.documentsService.getWordsIndex().then(words => {
        this.wordsIndex = words;
        resolve();
      }, reject);
    });
  }

  private updateDocuments(limit: number) {
    this.documentsService.getLatestProcessedDocuments(limit).subscribe(documents => {
      // TODO: Descobrir por que a primeira e segunda posição vem como ARRAY!!!!
      documents.forEach((item) => {
        if (item.fileName) {
          item.keywordsCount[0] = {0: item.keywordsCount[0][0]};
          item.keywordsCount[1] = {1: item.keywordsCount[1][1]};
        }
      });

      this._documents = documents;
      this.createGroups(this._documents);
    });
  }

  private createGroups(documents: DocumentModel[]) {
    this.learningService.compareOneAgainstAll(documents);
    this.groups = this.learningService.getGroups();
  }

  async ngOnInit() {
    await this.updateWordsIndex();
    this.updateDocuments(10);
    this.createGroups(this._documents);
  }

}
