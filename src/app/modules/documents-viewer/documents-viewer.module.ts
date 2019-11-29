import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ViewerComponent } from './components/viewer/viewer.component';
import { MenuComponent } from './components/menu/menu.component';
import { PdfViewerModule } from 'ng2-pdf-viewer';

@NgModule({
  declarations: [ViewerComponent, MenuComponent],
  imports: [
    CommonModule,
    PdfViewerModule
  ]
})
export class DocumentsViewerModule { }
