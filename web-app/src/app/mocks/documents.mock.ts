import { DocumentModel } from '../models/document.model';

export const DOCUMENTSMOCK: DocumentModel[] = [
  {
    fileName: 'lorem.pdf',
    fullText: 'Lorem ipsum dolor sit amet',
    keywordsCount: [
      {key: 0, count: 1},
      {key: 1, count: 1},
      {key: 2, count: 1},
      {key: 3, count: 1},
      {key: 4, count: 1}
    ]
  },
  {
    fileName: 'lorem2.pdf',
    fullText: 'Lorem lorem ipsum',
    keywordsCount: [
      {key: 0, count: 2},
      {key: 1, count: 1}
    ]
  }
];
