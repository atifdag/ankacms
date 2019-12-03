import { NgModule } from '@angular/core';
import { LanguageAddComponent } from './language-add/language-add.component';
import { LanguageListComponent } from './language-list/language-list.component';
import { LanguageUpdateComponent } from './language-update/language-update.component';
import { LanguageDetailComponent } from './language-detail/language-detail.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    LanguageAddComponent,
    LanguageListComponent,
    LanguageUpdateComponent,
    LanguageDetailComponent,
  ],
  imports: [
    SharedModule,
  ]
})
export class LanguageModule { }
