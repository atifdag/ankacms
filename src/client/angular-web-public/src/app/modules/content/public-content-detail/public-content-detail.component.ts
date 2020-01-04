import { Component, OnInit } from '@angular/core';
import { PublicContentModel } from 'src/app/models/public-content-model';
import { DublinCore } from 'src/app/value-objects/dublin-core';
import { TitleAndMetaService } from 'src/app/title-and-meta.service';
import { DublinCoreMetadataService } from 'src/app/dublin-core-metadata.service';
import { ContentService } from '../content.service';
import { MessageService } from 'src/app/primeng/components/common/api';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { AppSettingsService } from 'src/app/app-settings.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryService } from '../../category/category.service';
import { PublicCategoryModel } from 'src/app/models/public-category-model';

@Component({
  selector: 'app-public-content-detail',
  templateUrl: './public-content-detail.component.html',
  styleUrls: ['./public-content-detail.component.css']
})
export class PublicContentDetailComponent implements OnInit {

  loading = true;
  contentCode: string;
  pageTitle: string;
  dublinCore = new DublinCore();
  breadcrumbItems: any[];
  model = new PublicContentModel();
  categoryModel = new PublicCategoryModel();


  constructor(
    private serviceTitleAndMeta: TitleAndMetaService,
    private serviceDublinCoreMetadata: DublinCoreMetadataService,
    private serviceContent: ContentService,
    private messageService: MessageService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public appSettingsService: AppSettingsService,
    private route: ActivatedRoute,
    private serviceCategory: CategoryService,
    private router: Router
  ) { }

  ngOnInit() {

    this.route.params.subscribe(params => {
      this.contentCode = params.contentCode;
      this.getContentDetail(this.contentCode);
    });
  }

  getContentDetail(contentCode: string) {
    this.contentCode = contentCode;
    this.serviceContent.publicDetail(this.contentCode).subscribe(
      responseDetail => {
        if (responseDetail.status === 200) {
          this.model = responseDetail.body as PublicContentModel;
          this.pageTitle = this.model.shortName;

          if (this.model.url !== null && this.model.url !== '') {
            window.location.href = this.model.url;
          }

          this.breadcrumbItems = [
            { label: 'Ana Sayfa', routerLink: '/anasayfa' },
            { label: this.model.category.name, routerLink: '/sayfalar/' + this.model.category.code },
            { label: this.pageTitle }
          ];
          this.serviceTitleAndMeta.updateTitle(this.pageTitle);
          this.dublinCore.title = this.pageTitle;
          this.dublinCore.language = this.model.language.name;
          this.dublinCore.description = this.model.description;
          this.serviceDublinCoreMetadata.set(this.dublinCore);
          this.getCategoryDetail(this.model.category.code);

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


  getCategoryDetail(categoryCode: string) {
    this.serviceCategory.publicDetail(categoryCode).subscribe(
      res => {
        this.categoryModel = res.body as PublicCategoryModel;
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
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
