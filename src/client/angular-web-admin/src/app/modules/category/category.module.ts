import { NgModule } from '@angular/core';
import { CategoryAddComponent } from './category-add/category-add.component';
import { CategoryDetailComponent } from './category-detail/category-detail.component';
import { CategoryUpdateComponent } from './category-update/category-update.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    CategoryAddComponent,
    CategoryDetailComponent,
    CategoryUpdateComponent,
    CategoryListComponent
  ],
  imports: [
    SharedModule
  ]
})
export class CategoryModule { }
