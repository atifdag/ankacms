import { Component, OnInit } from '@angular/core';
import { AddModel } from 'src/app/models/add-model';
import { ContentModel } from 'src/app/models/content-model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SelectItem, MessageService } from 'src/app/primeng/components/common/api';
import { LanguageService } from '../../language/language.service';
import { CategoryService } from '../../category/category.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { ContentService } from '../content.service';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { Router } from '@angular/router';
import { IdCodeName } from 'src/app/value-objects/id-code-name';

@Component({
  selector: 'app-my-content-add',
  templateUrl: './my-content-add.component.html',
  styleUrls: ['./my-content-add.component.css']
})
export class MyContentAddComponent implements OnInit {
  disabledFieldset: boolean;
  model = new AddModel<ContentModel>();
  userForm: FormGroup;
  languages: SelectItem[];
  parentItems: SelectItem[];
  isApprovedChecked: boolean;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('MyContentManagement'), routerLink: '/Content/MyContentList' },
    { label: this.globalizationDictionaryPipe.transform('NewRecord') },
  ];

  constructor(
    private messageService: MessageService,
    private serviceLanguage: LanguageService,
    private serviceCategory: CategoryService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceContent: ContentService,
    private adminLayoutComponent: AdminLayoutComponent,
    private router: Router
  ) { }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new ContentModel();

    this.userForm = new FormGroup({
      selectedParentItem: new FormControl('', Validators.required),
      selectedLanguage: new FormControl('', Validators.required),
      code: new FormControl('', [Validators.required, Validators.minLength(2)]),
      name: new FormControl('', [Validators.required, Validators.minLength(2)]),
      shortName: new FormControl(''),
      description: new FormControl(''),
      keywords: new FormControl(''),
      contentDetail: new FormControl(''),
      url: new FormControl(''),
      file: new FormControl(''),
      isApproved: new FormControl(true)
    });

    this.serviceCategory.keysAndValues().subscribe(
      responseCategory => {
        if (responseCategory.status === 200) {

          const parentList = responseCategory.body as IdCodeName[];
          if (parentList.length > 0) {
            this.parentItems = [];
            this.parentItems.push({ label: this.globalizationDictionaryPipe.transform('Select'), value: '' });
            parentList.forEach(x => {
              this.parentItems.push(
                { value: x.id, label: x.name }
              );
            });
            this.serviceLanguage.keysAndValues().subscribe(
              responseLanguage => {
                if (responseLanguage.status === 200) {
                  const list = responseLanguage.body as IdCodeName[];
                  if (list.length > 0) {
                    this.languages = [];
                    this.languages.push({ label: this.globalizationDictionaryPipe.transform('Select'), value: '' });
                    list.forEach(x => {
                      this.languages.push(
                        { value: x.id, label: x.name }
                      );
                    });

                    this.serviceContent.myContentBeforeAdd().subscribe(
                      responseContent => {
                        if (responseContent.status === 200) {
                          this.model = responseContent.body as AddModel<ContentModel>;
                          if (this.model.item != null) {
                            this.userForm.get('code').setValue(this.model.item.code);
                            this.userForm.get('name').setValue(this.model.item.name);
                            this.userForm.get('shortName').setValue(this.model.item.shortName);
                            this.userForm.get('description').setValue(this.model.item.description);
                            this.userForm.get('keywords').setValue(this.model.item.keywords);
                            this.userForm.get('contentDetail').setValue(this.model.item.contentDetail);
                            this.userForm.get('url').setValue(this.model.item.url);
                            this.userForm.get('isApproved').setValue(this.model.item.isApproved);
                          }
                          this.disabledFieldset = false;
                        } else {
                          this.messageService.add({
                            severity: 'error',
                            summary: this.globalizationDictionaryPipe.transform('Error'),
                            detail: 'IN01 ' + responseContent.statusText
                          });
                        }
                      },
                      errorContent => {
                        if (errorContent.status === 400) {
                          if (errorContent.error != null) {
                            this.model.message = this.globalizationDictionaryPipe.transform('Error');
                            this.messageService.add({
                              severity: 'error',
                              summary: this.globalizationDictionaryPipe.transform('Error'),
                              detail: 'IN02 ' + this.model.message
                            });
                          } else {
                            this.model.message = errorContent.error;

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
                  } else {
                    this.messageService.add({
                      severity: 'error',
                      summary: this.globalizationDictionaryPipe.transform('Error'),
                      detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
                    });
                    setTimeout(() => {
                      this.router.navigate(['/Content/MyContentList']);
                    }, 1000);
                  }
                } else {
                  this.messageService.add({
                    severity: 'error',
                    summary: this.globalizationDictionaryPipe.transform('Error'),
                    detail: 'LI05 ' + responseLanguage.statusText
                  });
                }
              },
              errorLanguage => {
                if (errorLanguage.status === 400) {
                  if (errorLanguage.error != null) {
                    this.model.message = this.globalizationDictionaryPipe.transform('Error');
                    this.messageService.add({
                      severity: 'error',
                      summary: this.globalizationDictionaryPipe.transform('Error'),
                      detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
                    });
                    setTimeout(() => {
                      this.router.navigate(['/Language/List']);
                    }, 1000);
                  } else {
                    this.model.message = errorLanguage.error;
                    this.messageService.add({
                      severity: 'error',
                      summary: this.globalizationDictionaryPipe.transform('Error'),
                      detail: 'LI07 ' + this.model.message
                    });
                  }
                  setTimeout(() => {
                    this.router.navigate(['/Content/MyContentList']);
                  }, 1000);
                }
              }
            );

          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
            });
            setTimeout(() => {
              this.router.navigate(['/Content/MyContentList']);
            }, 1000);
          }
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'LI05 ' + responseCategory.statusText
          });
        }
      },
      errorCategory => {
        if (errorCategory.status === 400) {
          if (errorCategory.error != null) {
            this.model.message = this.globalizationDictionaryPipe.transform('Error');
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
            });
            setTimeout(() => {
              this.router.navigate(['/Category/List']);
            }, 1000);
          } else {
            this.model.message = errorCategory.error;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'LI07 ' + this.model.message
            });
          }
          setTimeout(() => {
            this.router.navigate(['/Category/List']);
          }, 1000);
        }
      }
    );
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
    this.messageService.clear();

    if (this.model.item == null) {
      this.model.item = new ContentModel();
    }
    if (this.model.item.category == null) {
      this.model.item.category = new IdCodeName();
    }
    this.model.item.category.id = this.f.selectedParentItem.value;



    if (this.model.item.language == null) {
      this.model.item.language = new IdCodeName();
    }
    this.model.item.language.id = this.f.selectedLanguage.value;
    this.model.item.code = this.f.code.value;
    this.model.item.name = this.f.name.value;
    this.model.item.shortName = this.f.shortName.value;
    this.model.item.description = this.f.description.value;
    this.model.item.keywords = this.f.keywords.value;
    this.model.item.contentDetail = this.f.contentDetail.value;
    this.model.item.url = this.f.url.value;
    this.model.item.isApproved = this.f.isApproved.value;


    if (this.userForm.get('file').value !== '') {
      this.model.item.file = this.userForm.get('file').value;
    }

    this.serviceContent.myContentAdd(this.model).subscribe(
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
            detail: res.statusText
          });
        }
      },
      err => {
        console.log(err);
        if (err.status === 400) {
          if (err.error != null) {
            this.model = err.error;
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: errors.toString()
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: err.error
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
