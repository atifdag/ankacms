import { Component, OnInit } from '@angular/core';
import { DetailModel } from 'src/app/models/detail-model';
import { ContentModel } from 'src/app/models/content-model';
import { FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { DatePipe } from '@angular/common';
import { MessageService } from 'src/app/primeng/components/common/api';
import { ContentService } from '../content.service';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { AppSettingsService } from 'src/app/app-settings.service';

@Component({
  selector: 'app-content-detail',
  templateUrl: './content-detail.component.html',
  styleUrls: ['./content-detail.component.css']
})
export class ContentDetailComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new DetailModel<ContentModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('ContentManagement'), routerLink: '/Content/List' },
    { label: this.globalizationDictionaryPipe.transform('Detail') },
  ];
  contentId: string;
  languageId: string;

  constructor(
    private route: ActivatedRoute,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private serviceContent: ContentService,
    public appSettingsService: AppSettingsService,
    private adminLayoutComponent: AdminLayoutComponent,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new ContentModel();
    this.userForm = new FormGroup({
      id: new FormControl(''),
      content: new FormControl(''),
      category: new FormControl(''),
      language: new FormControl(''),
      code: new FormControl(''),
      name: new FormControl(''),
      shortName: new FormControl(''),
      description: new FormControl(''),
      keywords: new FormControl(''),
      contentDetail: new FormControl(''),
      url: new FormControl(''),
      viewCount: new FormControl(''),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),
      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.route.paramMap.subscribe(params => {
      this.contentId = params.get('contentId');
      this.languageId = params.get('languageId');
      this.serviceContent.detail(this.contentId, this.languageId).subscribe(
        res => {
          this.loading = false;
          if (res.status === 200) {
            this.model = res.body as DetailModel<ContentModel>;
            if (this.model.item != null) {
              this.userForm.get('id').setValue(this.model.item.id);
              this.userForm.get('content').setValue(this.model.item.contentId);
              this.userForm.get('category').setValue(this.model.item.category.name);
              this.userForm.get('language').setValue(this.model.item.language.name);
              this.userForm.get('code').setValue(this.model.item.code);
              this.userForm.get('name').setValue(this.model.item.name);
              this.userForm.get('shortName').setValue(this.model.item.shortName);
              this.userForm.get('description').setValue(this.model.item.description);
              this.userForm.get('keywords').setValue(this.model.item.keywords);
              this.userForm.get('contentDetail').setValue(this.model.item.contentDetail);
              this.userForm.get('url').setValue(this.model.item.url);
              this.userForm.get('viewCount').setValue(this.model.item.viewCount);
              this.userForm.get('creator').setValue(this.model.item.creator.name);
              this.userForm.get('creationTime').setValue(
                this.datePipe.transform(this.model.item.creationTime, 'dd/MM/yyyy HH:mm:ss')
              );
              this.userForm.get('lastModifier').setValue(this.model.item.lastModifier.name);
              this.userForm.get('lastModificationTime').setValue(
                this.datePipe.transform(this.model.item.lastModificationTime, 'dd/MM/yyyy HH:mm:ss')
              );
              this.userForm.get('isApproved').setValue(this.model.item.isApproved);
              this.isApprovedChecked = this.model.item.isApproved;
            }
            this.disabledFieldset = false;
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN01 ' + res.statusText
            });
          }
        },
        err => {
          this.loading = false;
          if (err.status === 400) {
            if (err.error != null) {
              this.model.message = this.globalizationDictionaryPipe.transform('Error');
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN02 ' + this.model.message
              });
              setTimeout(() => {
                this.router.navigate(['/Content/List']);
              }, 1000);
            } else {
              this.model.message = err.error;
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN03 ' + this.model.message
              });
              setTimeout(() => {
                this.router.navigate(['/Content/List']);
              }, 3000);
            }
          }
        }
      );

    });
  }

  get f() {
    return this.userForm.controls;
  }

  backClick() {
    this.router.navigate(['/Content/List']);
  }

  updateClick() {
    this.router.navigate(['/Content/Update', { contentId: this.contentId, languageId: this.languageId }]);
  }

  onConfirmDelete() {
    this.messageService.clear('d');
    this.serviceContent.delete(this.contentId, this.languageId).subscribe(
      res => {
        if (res.status === 200) {
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoDeletionOperationSuccessful')
          });
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'CD01. ' + res.statusText
          });
        }
        setTimeout(() => {
          this.router.navigate(['/Content/List']);
        }, 3000);
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.model.message = errors.toString();
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'Kod: CD02. ' + this.model.message
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'Kod: CD03. ' + err.error
            });
          }
        }
        setTimeout(() => {
          this.router.navigate(['/Content/List']);
        }, 3000);
      }
    );
  }

  showConfirmDelete() {
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();
    this.messageService.add({
      key: 'd',
      sticky: true,
      severity: 'warn',
      summary: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
      detail: this.globalizationMessagesPipe.transform('QuestionAreYouSureDelete')
    });
  }


  onRejectDelete() {
    this.messageService.clear('d');
  }

  clear() {
    this.messageService.clear();
  }
}
