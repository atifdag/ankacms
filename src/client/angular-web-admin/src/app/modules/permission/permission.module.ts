import { NgModule } from '@angular/core';
import { PermissionListComponent } from './permission-list/permission-list.component';
import { PermissionAddComponent } from './permission-add/permission-add.component';
import { PermissionUpdateComponent } from './permission-update/permission-update.component';
import { PermissionDetailComponent } from './permission-detail/permission-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    PermissionListComponent,
    PermissionAddComponent,
    PermissionUpdateComponent,
    PermissionDetailComponent
  ],
  imports: [
    SharedModule,
  ]
})
export class PermissionModule { }
