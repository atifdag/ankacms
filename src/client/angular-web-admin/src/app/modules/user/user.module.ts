import { NgModule } from '@angular/core';
import { UserAddComponent } from './user-add/user-add.component';
import { UserUpdateComponent } from './user-update/user-update.component';
import { UserListComponent } from './user-list/user-list.component';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { UpdateMyPasswordComponent } from './update-my-password/update-my-password.component';
import { UpdateMyInformationComponent } from './update-my-information/update-my-information.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    MyProfileComponent,
    UserAddComponent,
    UserUpdateComponent,
    UserListComponent,
    UpdateMyPasswordComponent,
    UpdateMyInformationComponent,
    UserDetailComponent
  ],
  imports: [
    SharedModule,
  ],
})
export class UserModule { }
