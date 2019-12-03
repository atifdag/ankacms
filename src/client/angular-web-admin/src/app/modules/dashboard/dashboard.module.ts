import { NgModule } from '@angular/core';
import { DashboardIndexComponent } from './dashboard-index/dashboard-index.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [DashboardIndexComponent],
  imports: [
    SharedModule,
  ]
})
export class DashboardModule { }
