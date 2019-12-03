import { RouterModule, PreloadAllModules } from '@angular/router';
import { NgModule } from '@angular/core';
import { PublicLayoutComponent } from './layouts/public/public-layout/public-layout.component';
import { SignInLayoutComponent } from './layouts/sign-in/sign-in-layout/sign-in-layout.component';
import { SignInComponent } from './modules/authentication/sign-in/sign-in.component';
import { ForgotPasswordComponent } from './modules/authentication/forgot-password/forgot-password.component';
import { SignUpComponent } from './modules/authentication/sign-up/sign-up.component';
import { AdminLayoutComponent } from './layouts/admin/admin-layout/admin-layout.component';
import { DashboardIndexComponent } from './modules/dashboard/dashboard-index/dashboard-index.component';
import { AuthenticationGuard } from './authentication.guard';
import { CategoryAddComponent } from './modules/category/category-add/category-add.component';
import { CategoryListComponent } from './modules/category/category-list/category-list.component';
import { CategoryUpdateComponent } from './modules/category/category-update/category-update.component';
import { CategoryDetailComponent } from './modules/category/category-detail/category-detail.component';
import { ContentAddComponent } from './modules/content/content-add/content-add.component';
import { ContentListComponent } from './modules/content/content-list/content-list.component';
import { ContentUpdateComponent } from './modules/content/content-update/content-update.component';
import { ContentDetailComponent } from './modules/content/content-detail/content-detail.component';
import { MyContentAddComponent } from './modules/content/my-content-add/my-content-add.component';
import { MyContentListComponent } from './modules/content/my-content-list/my-content-list.component';
import { MyContentUpdateComponent } from './modules/content/my-content-update/my-content-update.component';
import { MyContentDetailComponent } from './modules/content/my-content-detail/my-content-detail.component';
import { LanguageAddComponent } from './modules/language/language-add/language-add.component';
import { LanguageListComponent } from './modules/language/language-list/language-list.component';
import { LanguageUpdateComponent } from './modules/language/language-update/language-update.component';
import { LanguageDetailComponent } from './modules/language/language-detail/language-detail.component';
import { MenuAddComponent } from './modules/menu/menu-add/menu-add.component';
import { MenuListComponent } from './modules/menu/menu-list/menu-list.component';
import { MenuUpdateComponent } from './modules/menu/menu-update/menu-update.component';
import { MenuDetailComponent } from './modules/menu/menu-detail/menu-detail.component';
import { ParameterAddComponent } from './modules/parameter/parameter-add/parameter-add.component';
import { ParameterListComponent } from './modules/parameter/parameter-list/parameter-list.component';
import { ParameterUpdateComponent } from './modules/parameter/parameter-update/parameter-update.component';
import { ParameterDetailComponent } from './modules/parameter/parameter-detail/parameter-detail.component';
import { ParameterGroupAddComponent } from './modules/parameter-group/parameter-group-add/parameter-group-add.component';
import { ParameterGroupListComponent } from './modules/parameter-group/parameter-group-list/parameter-group-list.component';
import { ParameterGroupUpdateComponent } from './modules/parameter-group/parameter-group-update/parameter-group-update.component';
import { ParameterGroupDetailComponent } from './modules/parameter-group/parameter-group-detail/parameter-group-detail.component';
import { PartAddComponent } from './modules/part/part-add/part-add.component';
import { PartListComponent } from './modules/part/part-list/part-list.component';
import { PartUpdateComponent } from './modules/part/part-update/part-update.component';
import { PartDetailComponent } from './modules/part/part-detail/part-detail.component';
import { PermissionAddComponent } from './modules/permission/permission-add/permission-add.component';
import { PermissionListComponent } from './modules/permission/permission-list/permission-list.component';
import { PermissionUpdateComponent } from './modules/permission/permission-update/permission-update.component';
import { PermissionDetailComponent } from './modules/permission/permission-detail/permission-detail.component';
import { RoleAddComponent } from './modules/role/role-add/role-add.component';
import { RoleListComponent } from './modules/role/role-list/role-list.component';
import { RoleUpdateComponent } from './modules/role/role-update/role-update.component';
import { RoleDetailComponent } from './modules/role/role-detail/role-detail.component';
import { UserAddComponent } from './modules/user/user-add/user-add.component';
import { UserListComponent } from './modules/user/user-list/user-list.component';
import { UserUpdateComponent } from './modules/user/user-update/user-update.component';
import { UserDetailComponent } from './modules/user/user-detail/user-detail.component';
import { MyProfileComponent } from './modules/user/my-profile/my-profile.component';
import { UpdateMyPasswordComponent } from './modules/user/update-my-password/update-my-password.component';
import { UpdateMyInformationComponent } from './modules/user/update-my-information/update-my-information.component';
import { HomeComponent } from './components/home/home.component';
import { CacheListComponent } from './modules/cache/cache-list/cache-list.component';

@NgModule({
    imports: [
        RouterModule.forRoot(
            [
                {
                    path: '',
                    component: PublicLayoutComponent,
                    children: [
                        { path: '', component: HomeComponent, pathMatch: 'full' },
                        { path: 'Home', component: HomeComponent },
                    ]
                },
                {
                    path: '',
                    component: SignInLayoutComponent,
                    children: [
                        { path: 'Authentication', component: SignInComponent },
                        { path: 'Authentication/SignIn', component: SignInComponent },
                        { path: 'Authentication/ForgotPassword', component: ForgotPasswordComponent },
                        { path: 'Authentication/SignUp', component: SignUpComponent }
                    ]
                },
                {
                    path: '',
                    component: AdminLayoutComponent,
                    children: [

                        { path: 'Dashboard/Index', component: DashboardIndexComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Cache/List', component: CacheListComponent, canActivate: [AuthenticationGuard] },


                        { path: 'Category/Add', component: CategoryAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Category/List', component: CategoryListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Category/Update', component: CategoryUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Category/Detail', component: CategoryDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Content/Add', component: ContentAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Content/List', component: ContentListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Content/Update', component: ContentUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Content/Detail', component: ContentDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Content/MyContentAdd', component: MyContentAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Content/MyContentList', component: MyContentListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Content/MyContentUpdate', component: MyContentUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Content/MyContentDetail', component: MyContentDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'ParameterGroup/Add', component: ParameterGroupAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'ParameterGroup/List', component: ParameterGroupListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'ParameterGroup/Update', component: ParameterGroupUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'ParameterGroup/Detail', component: ParameterGroupDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Language/Add', component: LanguageAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Language/List', component: LanguageListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Language/Update', component: LanguageUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Language/Detail', component: LanguageDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Menu/Add', component: MenuAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Menu/List', component: MenuListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Menu/Update', component: MenuUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Menu/Detail', component: MenuDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Parameter/Add', component: ParameterAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Parameter/List', component: ParameterListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Parameter/Update', component: ParameterUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Parameter/Detail', component: ParameterDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'ParameterGroup/Add', component: ParameterGroupAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'ParameterGroup/List', component: ParameterGroupListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'ParameterGroup/Update', component: ParameterGroupUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'ParameterGroup/Detail', component: ParameterGroupDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Part/Add', component: PartAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Part/List', component: PartListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Part/Update', component: PartUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Part/Detail', component: PartDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Permission/Add', component: PermissionAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Permission/List', component: PermissionListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Permission/Update', component: PermissionUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Permission/Detail', component: PermissionDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'Role/Add', component: RoleAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Role/List', component: RoleListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Role/Update', component: RoleUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'Role/Detail', component: RoleDetailComponent, canActivate: [AuthenticationGuard] },

                        { path: 'User/Add', component: UserAddComponent, canActivate: [AuthenticationGuard] },
                        { path: 'User/List', component: UserListComponent, canActivate: [AuthenticationGuard] },
                        { path: 'User/Update', component: UserUpdateComponent, canActivate: [AuthenticationGuard] },
                        { path: 'User/Detail', component: UserDetailComponent, canActivate: [AuthenticationGuard] },
                        { path: 'User/MyProfile', component: MyProfileComponent, canActivate: [AuthenticationGuard] },
                        { path: 'User/UpdateMyPassword', component: UpdateMyPasswordComponent, canActivate: [AuthenticationGuard] },
                        { path: 'User/UpdateMyInformation', component: UpdateMyInformationComponent, canActivate: [AuthenticationGuard] },
                    ]
                },
                { path: '**', redirectTo: '' }
            ], {
                preloadingStrategy: PreloadAllModules
            }
        ),
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }