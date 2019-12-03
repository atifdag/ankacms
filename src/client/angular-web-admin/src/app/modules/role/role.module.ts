import { NgModule } from '@angular/core';
import { RoleAddComponent } from './role-add/role-add.component';
import { RoleUpdateComponent } from './role-update/role-update.component';
import { RoleListComponent } from './role-list/role-list.component';
import { RoleDetailComponent } from './role-detail/role-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    RoleAddComponent,
    RoleUpdateComponent,
    RoleListComponent,
    RoleDetailComponent
  ],
  imports: [
    SharedModule
  ]
})
export class RoleModule { }
