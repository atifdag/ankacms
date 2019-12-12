import { RouterModule, PreloadAllModules } from '@angular/router';
import { NgModule } from '@angular/core';
import { HomeIndexComponent } from './modules/home/home-index/home-index.component';

@NgModule({
    imports: [
        RouterModule.forRoot(
            [
                { path: '', component: HomeIndexComponent, pathMatch: 'full' },
                { path: 'anasayfa', component: HomeIndexComponent },
                { path: '**', redirectTo: '' }
            ], {
                preloadingStrategy: PreloadAllModules
            }
        ),
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }