import { NgModule } from '@angular/core';
import { ParameterAddComponent } from './parameter-add/parameter-add.component';
import { ParameterUpdateComponent } from './parameter-update/parameter-update.component';
import { ParameterListComponent } from './parameter-list/parameter-list.component';
import { ParameterDetailComponent } from './parameter-detail/parameter-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    ParameterAddComponent,
    ParameterUpdateComponent,
    ParameterListComponent,
    ParameterDetailComponent
  ],
  imports: [
    SharedModule,
  ]
})
export class ParameterModule { }
