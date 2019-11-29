import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ViewerComponent } from './components/viewer/viewer.component';
import { MenuComponent } from './components/menu/menu.component';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { environment } from 'src/environments/environment';
import { AngularFireModule } from '@angular/fire';
import { AngularFireStorageModule } from '@angular/fire/storage';
import { AngularFirestoreModule } from '@angular/fire/firestore';
import { AngularFireDatabaseModule } from '@angular/fire/database';

@NgModule({
  declarations: [ViewerComponent, MenuComponent],
  imports: [
    CommonModule,
    PdfViewerModule,
    AngularFirestoreModule,
    AngularFireStorageModule,
    AngularFireDatabaseModule,
    AngularFireModule.initializeApp(environment.firebase)
  ]
})
export class DocumentsViewerModule { }
