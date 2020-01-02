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

@Component({
  selector: 'app-public-content-detail',
  templateUrl: './public-content-detail.component.html',
  styleUrls: ['./public-content-detail.component.css']
})
export class PublicContentDetailComponent implements OnInit {

  loading = true;
  pageCode: string;
  pageTitle: string;
  dublinCore = new DublinCore();
  breadcrumbItems: any[];
  model = new PublicContentModel();


  constructor(
    private serviceTitleAndMeta: TitleAndMetaService,
    private serviceDublinCoreMetadata: DublinCoreMetadataService,
    private serviceContent: ContentService,
    private messageService: MessageService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public appSettingsService: AppSettingsService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.pageCode = params.get('code');
    });
    this.getContentDetail(this.pageCode);
  }

  getContentDetail(code: string) {
    this.pageCode = code;
    this.serviceContent.publicDetail(this.pageCode).subscribe(
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
