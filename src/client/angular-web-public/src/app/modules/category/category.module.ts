import { NgModule } from '@angular/core';
import { PublicCategoryDetailComponent } from './public-category-detail/public-category-detail.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [PublicCategoryDetailComponent],
  imports: [
    SharedModule
  ]
})
export class CategoryModule { }
