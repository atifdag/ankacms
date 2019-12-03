import { Component, OnInit } from '@angular/core';
import { UpdateModel } from 'src/app/models/update-model';
import { ContentModel } from 'src/app/models/content-model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { ContentService } from '../content.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { AppSettingsService } from 'src/app/app-settings.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-my-content-update',
  templateUrl: './my-content-update.component.html',
  styleUrls: ['./my-content-update.component.css']
})
export class MyContentUpdateComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new UpdateModel<ContentModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('MyContentManagement'), routerLink: '/Content/MyContentList' },
    { label: this.globalizationDictionaryPipe.transform('Update') },
  ];
  contentId: string;
  languageId: string;


  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    private serviceContent: ContentService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    public appSettingsService: AppSettingsService,
    private datePipe: DatePipe,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new ContentModel();
    this.userForm = new FormGroup({
      content: new FormControl(''),
      category: new FormControl(''),
      language: new FormControl(''),
      code: new FormControl('', Validators.required),
      name: new FormControl('', [Validators.required, Validators.minLength(2)]),
      shortName: new FormControl(''),
      description: new FormControl(''),
      keywords: new FormControl(''),
      contentDetail: new FormControl(''),
      url: new FormControl(''),
      file: new FormControl(''),
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
    });

    this.serviceContent.myContentBeforeUpdate(this.contentId, this.languageId).subscribe(
      res => {
        this.loading = false;
        if (res.status === 200) {
          this.model = res.body as UpdateModel<ContentModel>;
          if (this.model.item != null) {
            console.log(this.model);

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
          } else {
            this.model.message = err.error;

            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN03 ' + this.model.message
            });
          }
          setTimeout(() => {
            this.router.navigate(['/Content/MyContentList']);
          }, 1000);
        }
      }
    );
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }

  get f() {
    return this.userForm.controls;
  }


  onFileChange(event) {


    const reader = new FileReader();
    if (event.files && event.files.length > 0) {
      let file = event.files[0];
      if (!file.name.match(/(\.jpg|\.png|\.JPG|\.PNG|\.jpeg|\.JPEG)$/)) {
        this.model.message = this.globalizationMessagesPipe.transform('DangerInvalidUploadJpgPng');
        this.messageService.add({
          severity: 'error',
          summary: this.globalizationDictionaryPipe.transform('Error'),
          detail: 'onUpload 1 ' + this.model.message
        });
        this.userForm.get('file').setValue(null);
      } else {
        if (file.size > 1024 * 1024 * 1) {
          this.model.message = this.globalizationMessagesPipe.transform('DangerInvalidFileSize1MB');
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'onUpload 2 ' + this.model.message
          });
          this.userForm.get('file').setValue(null);
        } else {
          reader.readAsDataURL(file);
          reader.onload = () => {
            this.userForm.get('file').setValue({
              lastModified: file.lastModified,
              name: file.name,
              size: file.size,
              type: file.type,
              value: reader.result
            });
          };
        }


      }

    }
  }

  onSubmit() {

    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.adminLayoutComponent.resetCounter();
    if (this.model.item == null) {
      this.model.item = new ContentModel();
    }
    this.messageService.clear();

    this.model.item.code = this.f.code.value;
    this.model.item.name = this.f.name.value;
    this.model.item.shortName = this.f.shortName.value;
    this.model.item.description = this.f.description.value;
    this.model.item.keywords = this.f.keywords.value;
    this.model.item.contentDetail = this.f.contentDetail.value;
    this.model.item.url = this.f.url.value;

    if (this.userForm.get('file').value !== '') {
      this.model.item.file = this.userForm.get('file').value;
    }

    this.serviceContent.myContentUpdate(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            this.router.navigate(['/Content/MyContentList']);
          }, 1000);
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'SU01 ' + res.statusText
          });
        }
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {

            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'SU02 ' + errors.toString()
            });
          } else {
            this.model.message = err.error;

            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'SU03 ' + this.model.message
            });
          }
        }
      }
    );
  }

  backClick() {
    this.router.navigate(['/Content/MyContentList']);
  }

}
