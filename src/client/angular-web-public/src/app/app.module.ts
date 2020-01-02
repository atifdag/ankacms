import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { SharedModule } from './modules/shared/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { MessageService } from './primeng/components/common/api';
import { PublicLayoutComponent } from './layouts/public/public-layout/public-layout.component';
import { HomeModule } from './modules/home/home.module';
import { PartModule } from './modules/part/part.module';
import { ContentModule } from './modules/content/content.module';
import { CategoryModule } from './modules/category/category.module';

@NgModule({
  declarations: [
    AppComponent,
    PublicLayoutComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SharedModule,
    HomeModule,
    PartModule,
    ContentModule,
    CategoryModule,

  ],
  providers: [
    // { provide: LocationStrategy, useClass: HashLocationStrategy },
    MessageService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule {

}

