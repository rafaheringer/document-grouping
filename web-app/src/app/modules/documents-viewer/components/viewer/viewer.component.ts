import { Component, OnInit } from '@angular/core';
import { DocumentModel, GlobalKeywordsModel } from 'src/app/models/document.model';
import { DocumentsService } from '../../services/documents.service';
import { LearningService } from '../../services/learning.service';
import { GroupLearningModel } from 'src/app/models/learning.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-viewer',
  templateUrl: './viewer.component.html',
  styleUrls: ['./viewer.component.sass']
})
export class ViewerComponent implements OnInit {

  public groups: GroupLearningModel[] = [];
  public wordsIndex: GlobalKeywordsModel[] = [];
  public states = {loadingWords: true, loadingDocuments: true};

  private _documents: DocumentModel[] = [];

  public getWordByIndex(index: string) {
    // tslint:disable-next-line: radix
    let i = parseInt(index);
    return this.wordsIndex.find(a => a.Key === i).Value;
  }

  constructor(
    private documentsService: DocumentsService,
    private learningService: LearningService
    ) {}


  private createGroups(documents: DocumentModel[]) {
    this.learningService.compareOneAgainstAll(documents);
    this.groups = this.learningService.getGroups();
    console.log('Groups created...');
  }

  async ngOnInit() {
    this.states.loadingWords = true;
    this.states.loadingDocuments = true;

    this.documentsService.getWordsIndex().subscribe(words => {
      this.wordsIndex = words;
      this.states.loadingWords = false;
      console.log('Updated words index...');

      this.documentsService.getLatestProcessedDocuments(10).subscribe(documents => {
        this._documents = documents;
        this.states.loadingDocuments = false;
        console.log('Updated documents...');

        this.createGroups(this._documents);
      });
    });
  }

}
