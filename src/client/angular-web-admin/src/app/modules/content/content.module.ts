import { NgModule } from '@angular/core';
import { ContentAddComponent } from './content-add/content-add.component';
import { ContentDetailComponent } from './content-detail/content-detail.component';
import { ContentUpdateComponent } from './content-update/content-update.component';
import { ContentListComponent } from './content-list/content-list.component';
import { SharedModule } from '../shared/shared.module';
import { MyContentListComponent } from './my-content-list/my-content-list.component';
import { MyContentAddComponent } from './my-content-add/my-content-add.component';
import { MyContentUpdateComponent } from './my-content-update/my-content-update.component';
import { MyContentDetailComponent } from './my-content-detail/my-content-detail.component';



@NgModule({
  declarations: [
    ContentAddComponent,
    ContentDetailComponent,
    ContentUpdateComponent,
    ContentListComponent,
    MyContentListComponent,
    MyContentAddComponent,
    MyContentUpdateComponent,
    MyContentDetailComponent,
  ],
  imports: [
    SharedModule
  ]
})
export class ContentModule { }
