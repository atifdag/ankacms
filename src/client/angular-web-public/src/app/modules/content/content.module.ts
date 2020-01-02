import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { PublicContentDetailComponent } from './public-content-detail/public-content-detail.component';



@NgModule({
  declarations: [
    PublicContentDetailComponent
  ],
  imports: [
    SharedModule
  ]
})
export class ContentModule { }
