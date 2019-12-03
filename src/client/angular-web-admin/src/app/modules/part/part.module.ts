import { NgModule } from '@angular/core';
import { PartAddComponent } from './part-add/part-add.component';
import { PartDetailComponent } from './part-detail/part-detail.component';
import { PartUpdateComponent } from './part-update/part-update.component';
import { PartListComponent } from './part-list/part-list.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    PartAddComponent,
    PartDetailComponent,
    PartUpdateComponent,
    PartListComponent
  ],
  imports: [
    SharedModule
  ]
})
export class PartModule { }
