import { RouterModule, PreloadAllModules } from '@angular/router';
import { NgModule } from '@angular/core';
import { HomeIndexComponent } from './modules/home/home-index/home-index.component';
import { PublicLayoutComponent } from './layouts/public/public-layout/public-layout.component';
import { PublicCategoryDetailComponent } from './modules/category/public-category-detail/public-category-detail.component';
import { PublicContentDetailComponent } from './modules/content/public-content-detail/public-content-detail.component';

@NgModule({
    imports: [
        RouterModule.forRoot(
            [
                {
                    path: '',
                    component: PublicLayoutComponent,
                    children: [
                        {
                            path: '',
                            component: HomeIndexComponent,
                            pathMatch: 'full'
                        },
                        {
                            path: 'anasayfa',
                            component: HomeIndexComponent
                        },
                        {
                            path: 'sayfalar/:categoryCode',
                            component: PublicCategoryDetailComponent,

                        },
                        {
                            path: 'sayfalar/:categoryCode/:contentCode',
                            component: PublicContentDetailComponent
                        },
                    ],
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