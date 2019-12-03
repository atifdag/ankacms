import { NgModule } from '@angular/core';
import { ParameterGroupListComponent } from './parameter-group-list/parameter-group-list.component';
import { ParameterGroupAddComponent } from './parameter-group-add/parameter-group-add.component';
import { ParameterGroupUpdateComponent } from './parameter-group-update/parameter-group-update.component';
import { ParameterGroupDetailComponent } from './parameter-group-detail/parameter-group-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    ParameterGroupListComponent,
    ParameterGroupAddComponent,
    ParameterGroupUpdateComponent,
    ParameterGroupDetailComponent
  ],
  imports: [
    SharedModule,
  ]
})
export class ParameterGroupModule { }
