import { Component, OnInit, OnDestroy } from '@angular/core';
import { DublinCore } from 'src/app/value-objects/dublin-core';
import { PublicCategoryModel } from 'src/app/models/public-category-model';
import { TitleAndMetaService } from 'src/app/title-and-meta.service';
import { DublinCoreMetadataService } from 'src/app/dublin-core-metadata.service';
import { CategoryService } from '../category.service';
import { MessageService } from 'src/app/primeng/components/common/api';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { AppSettingsService } from 'src/app/app-settings.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-public-category-detail',
  templateUrl: './public-category-detail.component.html',
  styleUrls: ['./public-category-detail.component.css']
})
export class PublicCategoryDetailComponent implements OnInit {

  loading = true;
  categoryCode: string;
  pageTitle: string;
  dublinCore = new DublinCore();
  breadcrumbItems: any[];
  model = new PublicCategoryModel();

  constructor(
    private serviceTitleAndMeta: TitleAndMetaService,
    private serviceDublinCoreMetadata: DublinCoreMetadataService,
    private serviceCategory: CategoryService,
    private messageService: MessageService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public appSettingsService: AppSettingsService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.categoryCode = params.categoryCode;
      this.getCategoryDetail(this.categoryCode);
    });
  }

  getCategoryDetail(categoryCode: string) {
    this.categoryCode = categoryCode;
    this.serviceCategory.publicDetail(this.categoryCode).subscribe(
      responseDetail => {
        if (responseDetail.status === 200) {
          this.model = responseDetail.body as PublicCategoryModel;
          this.pageTitle = this.model.name;
          this.breadcrumbItems = [
            { label: 'Ana Sayfa', routerLink: '/anasayfa' },
            { label: this.pageTitle }
          ];
          this.serviceTitleAndMeta.updateTitle(this.pageTitle);
          this.dublinCore.title = this.pageTitle;
          this.dublinCore.language = this.model.language.name;
          this.dublinCore.description = this.model.description;
          this.serviceDublinCoreMetadata.set(this.dublinCore);

        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'IN01 ' + responseDetail.statusText
          });
        }
        this.loading = false;
      },
      errorDetail => {
        if (errorDetail.status === 400) {
          if (errorDetail.error != null) {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'Hata oluştu.'
            });

          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'Hata Oluştu.'
            });
          }
          this.loading = false;
          // setTimeout(() => {
          //   this.router.navigate(['/anasayfa']);
          // }, 3000);
        }
      }
    );
  }

}
