import { NgModule } from '@angular/core';
import { CacheListComponent } from './cache-list/cache-list.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [CacheListComponent],
  imports: [
    SharedModule
  ]
})
export class CacheModule { }
