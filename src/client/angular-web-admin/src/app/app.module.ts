import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { SharedModule } from './modules/shared/shared.module';
import { AuthenticationModule } from './modules/authentication/authentication.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AdminLayoutComponent } from './layouts/admin/admin-layout/admin-layout.component';
import { AuthenticationInterceptor } from './authentication-interceptor';
import { MessageService } from './primeng/components/common/api';
import { TimeCountDownPipe } from './pipes/time-count-down.pipe';
import { CategoryModule } from './modules/category/category.module';
import { ContentModule } from './modules/content/content.module';
import { LanguageModule } from './modules/language/language.module';
import { ParameterModule } from './modules/parameter/parameter.module';
import { ParameterGroupModule } from './modules/parameter-group/parameter-group.module';
import { PartModule } from './modules/part/part.module';
import { PermissionModule } from './modules/permission/permission.module';
import { RoleModule } from './modules/role/role.module';
import { UserModule } from './modules/user/user.module';
import { MenuModule } from './modules/menu/menu.module';
import { DashboardModule } from './modules/dashboard/dashboard.module';
import { SignInHeaderComponent } from './layouts/sign-in/sign-in-header/sign-in-header.component';
import { SignInFooterComponent } from './layouts/sign-in/sign-in-footer/sign-in-footer.component';
import { SignInLayoutComponent } from './layouts/sign-in/sign-in-layout/sign-in-layout.component';
import { HomeComponent } from './components/home/home.component';
import { PublicLayoutComponent } from './layouts/public/public-layout/public-layout.component';
import { PublicHeaderComponent } from './layouts/public/public-header/public-header.component';
import { PublicFooterComponent } from './layouts/public/public-footer/public-footer.component';
import { CacheModule } from './modules/cache/cache.module';

@NgModule({
  declarations: [
    AppComponent,
    PublicLayoutComponent,
    PublicHeaderComponent,
    PublicFooterComponent,
    AdminLayoutComponent,
    HomeComponent,
    TimeCountDownPipe,
    SignInHeaderComponent,
    SignInFooterComponent,
    SignInLayoutComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SharedModule,
    AuthenticationModule,
    CacheModule,
    CategoryModule,
    ContentModule,
    DashboardModule,
    LanguageModule,
    MenuModule,
    ParameterModule,
    ParameterGroupModule,
    PartModule,
    PermissionModule,
    RoleModule,
    UserModule,
  ],
  providers: [
    // { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: HTTP_INTERCEPTORS, useClass: AuthenticationInterceptor, multi: true },
    MessageService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule {

}

