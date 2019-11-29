import { Component, OnInit } from '@angular/core';
import { DocumentModel } from 'src/app/models/document.model';
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
  public wordsIndex: { [key: number]: number } = [];

  private _documents: DocumentModel[] = [];

  constructor(
    private documentsService: DocumentsService,
    private learningService: LearningService
    ) { }

  private updateWordsIndex() {
    this.documentsService.getWordsIndex().subscribe(words => {
      this.wordsIndex = words;
    });
  }

  private updateDocuments(limit: number) {
    this.documentsService.getLatestProcessedDocuments(limit).subscribe(documents => {
      this._documents = documents;
    });
  }

  private createGroups(documents: DocumentModel[]) {
    this.learningService.compareOneAgainstAll(documents);
    this.groups = this.learningService.getGroups();
  }

  ngOnInit() {
    this.updateWordsIndex();
    this.updateDocuments(10);
    this.createGroups(this._documents);
  }

}
