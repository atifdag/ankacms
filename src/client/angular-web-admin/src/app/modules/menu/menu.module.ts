import { NgModule } from '@angular/core';
import { MenuAddComponent } from './menu-add/menu-add.component';
import { MenuListComponent } from './menu-list/menu-list.component';
import { MenuUpdateComponent } from './menu-update/menu-update.component';
import { MenuDetailComponent } from './menu-detail/menu-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    MenuAddComponent,
    MenuListComponent,
    MenuUpdateComponent,
    MenuDetailComponent
  ],
  imports: [
    SharedModule,
  ]
})
export class MenuModule { }
