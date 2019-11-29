import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ViewerComponent } from './modules/documents-viewer/components/viewer/viewer.component';

const routes: Routes = [
  {
    path: '',
    component: ViewerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
