import { Component, OnInit } from '@angular/core';
import { SelectItem, MenuItem, MessageService } from 'src/app/primeng/components/common/api';
import { FormGroup, FormControl } from '@angular/forms';
import { ListModel } from 'src/app/models/list-model';
import { ContentModel } from 'src/app/models/content-model';
import { FilterModelWithLanguageAndParent } from 'src/app/models/filter-model-with-language-and-parent';
import { AppSettingsService } from 'src/app/app-settings.service';
import { LanguageService } from '../../language/language.service';
import { CategoryService } from '../../category/category.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { ContentService } from '../content.service';
import { Router } from '@angular/router';
import { KeyValue } from '@angular/common';
import { IdCodeName } from 'src/app/value-objects/id-code-name';

@Component({
  selector: 'app-my-content-list',
  templateUrl: './my-content-list.component.html',
  styleUrls: ['./my-content-list.component.css']
})
export class MyContentListComponent implements OnInit {

  loading = false;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('MyContentManagement'), routerLink: '/Content/MyContentList' },
  ];

  parentItems: SelectItem[];
  languages: SelectItem[];
  yearRange: string;
  submitted: boolean;
  statusOptions: any[];
  pageSizes: any[];
  userForm: FormGroup;
  listModel = new ListModel<ContentModel>();
  filterModel = new FilterModelWithLanguageAndParent();
  selectedPageNumber = 1;
  selectedPageSize = this.appSettingsService.selectedPageSize.key;
  rowsPerPageOptions = this.appSettingsService.rowsPerPageOptions;
  tr = this.appSettingsService.calendarTr;
  tableOps(contentId: string, languageId: string): MenuItem[] {
    return [
      {
        label: this.globalizationDictionaryPipe.transform('Detail'), icon: 'pi pi-eye', command: () => {
          this.detail(contentId, languageId);
        }
      },
      {
        label: this.globalizationDictionaryPipe.transform('Update'), icon: 'pi pi-pencil', command: () => {
          this.update(contentId, languageId);
        }
      },
      {
        label: this.globalizationDictionaryPipe.transform('Delete'), icon: 'pi pi-trash', command: () => {
          this.showConfirmDelete(contentId, languageId);
        }
      }
    ];
  }

  constructor(
    private messageService: MessageService,
    public appSettingsService: AppSettingsService,
    private serviceLanguage: LanguageService,
    private serviceCategory: CategoryService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    private serviceContent: ContentService,
    private router: Router
  ) {
    const startDate = new Date();
    const endDate = new Date();
    const today = new Date();
    const year = today.getFullYear();
    const invalidDate = new Date();
    invalidDate.setDate(today.getDate() - 1);
    this.yearRange = (year - 10).toString() + ':' + year.toString();
    startDate.setFullYear(today.getFullYear() - 2);
    this.statusOptions = this.appSettingsService.statusOptions;
    this.pageSizes = this.appSettingsService.pageSizes;
    this.filterModel.startDate = startDate;
    this.filterModel.endDate = endDate;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;
  }



  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.loading = true;
    this.userForm = new FormGroup({
      searched: new FormControl(this.filterModel.searched),
      startDate: new FormControl(this.filterModel.startDate),
      endDate: new FormControl(this.filterModel.endDate),
      selectedStatus: new FormControl(this.appSettingsService.selectedStatus),
      selectedParentItem: new FormControl(),
      selectedLanguage: new FormControl()
    });

    this.serviceCategory.keysAndValues().subscribe(
      responseCategory => {
        if (responseCategory.status === 200) {

          const listCategory = responseCategory.body as IdCodeName[];
          if (listCategory.length > 0) {

            this.parentItems = [];
            this.parentItems.push({ label: this.globalizationDictionaryPipe.transform('Select'), value: '' });
            listCategory.forEach(x => {
              this.parentItems.push(
                { value: x.id, label: x.name }
              );
            });

            this.serviceLanguage.keysAndValues().subscribe(
              responseLanguage => {
                if (responseLanguage.status === 200) {
                  const listLanguage = responseLanguage.body as IdCodeName[];
                  if (listLanguage.length > 0) {
                    this.languages = [];
                    this.languages.push({ label: this.globalizationDictionaryPipe.transform('Select'), value: '' });
                    listLanguage.forEach(x => {
                      this.languages.push(
                        { value: x.id, label: x.name }
                      );
                    });
                    this.list();
                  } else {
                    this.messageService.add({
                      severity: 'error',
                      summary: this.globalizationDictionaryPipe.transform('Error'),
                      detail: 'IN01 ' + responseLanguage.statusText
                    });
                  }

                } else {
                  this.messageService.add({
                    severity: 'error',
                    summary: this.globalizationDictionaryPipe.transform('Error'),
                    detail: 'IN02 ' + this.globalizationMessagesPipe.transform('DangerParentNotFound')
                  });
                }
              },
              errorLanguage => {
                if (errorLanguage.status === 400) {
                  if (errorLanguage.error != null) {
                    this.listModel.message = this.globalizationDictionaryPipe.transform('Error');
                    this.listModel.hasError = true;
                    this.messageService.add({
                      severity: 'error',
                      summary: this.globalizationDictionaryPipe.transform('Error'),
                      detail: 'IN03 ' + this.listModel.message
                    });
                  } else {
                    this.listModel.message = errorLanguage.error;
                    this.listModel.hasError = true;
                    this.messageService.add({
                      severity: 'error',
                      summary: this.globalizationDictionaryPipe.transform('Error'),
                      detail: 'IN04 ' + this.listModel.message
                    });
                  }
                  this.loading = false;
                  setTimeout(() => {
                    this.router.navigate(['/Language/List']);
                  }, 1000);
                }
              }
            );
          }
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'IN05 ' + this.globalizationMessagesPipe.transform('DangerParentNotFound')
          });
        }

      },
      errorCategory => {
        if (errorCategory.status === 400) {
          if (errorCategory.error != null) {
            this.listModel.message = this.globalizationDictionaryPipe.transform('Error');
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN06 ' + this.listModel.message
            });
          } else {
            this.listModel.message = errorCategory.error;
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN07 ' + this.listModel.message
            });
          }
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/Category/List']);
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

  paginate(event) {
    this.selectedPageNumber = event.page + 1;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.selectedPageSize = event.rows;
    this.filterModel.pageSize = this.selectedPageSize;
    this.changeForm();
  }

  list() {



    this.serviceContent.myContentList().subscribe(

      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<ContentModel>;
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'LI01 ' + res.statusText
          });
        }
        this.loading = false;
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.listModel.message = this.globalizationDictionaryPipe.transform('Error');
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'LI02 ' + this.listModel.message
            });
          } else {
            this.listModel.message = err.error;
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'LI03 ' + this.listModel.message
            });
          }
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/Content/MyContentList']);
          }, 1000);
        }
      }
    );
  }

  changeForm() {
    this.submitted = true;
    this.messageService.clear();
    this.loading = true;
    this.filterModel.searched = this.f.searched.value;
    this.filterModel.startDate = this.f.startDate.value;
    this.filterModel.endDate = this.f.endDate.value;
    this.filterModel.status = this.f.selectedStatus.value.key;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;

    if (this.f.selectedLanguage.value != null && this.f.selectedLanguage.value.length > 0) {
      this.filterModel.language.id = this.f.selectedLanguage.value;

    } else {
      this.filterModel.language = new IdCodeName();

    }

    if (this.f.selectedParentItem.value != null && this.f.selectedParentItem.value.length > 0) {
      this.filterModel.parent.id = this.f.selectedParentItem.value;

    } else {
      this.filterModel.parent = new IdCodeName();

    }
    this.filter();
  }

  get f() {
    return this.userForm.controls;
  }

  filter() {
    this.adminLayoutComponent.resetCounter();
    this.serviceContent.myContentFilter(this.filterModel).subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<ContentModel>;
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'FI01 ' + res.statusText
          });
        }
        this.loading = false;
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.listModel.message = this.globalizationDictionaryPipe.transform('Error');
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'FI02 ' + this.listModel.message
            });
          } else {
            this.listModel.message = err.error;
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'FI03 ' + this.listModel.message
            });
          }
          setTimeout(() => {
            this.router.navigate(['/Content/MyContentList']);
          }, 1000);
        }
      }
    );
  }

  detail(contentId: string, languageId: string) {
    this.router.navigate(['/Content/MyContentDetail', { contentId, languageId }]);
  }

  showConfirmDelete(contentId: string, languageId: string) {
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();
    this.messageService.add({
      key: 'd',
      sticky: true,
      data: { contentId, languageId },
      severity: 'warn',
      summary: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
      detail: this.globalizationMessagesPipe.transform('QuestionAreYouSureDelete')
    });
  }

  onConfirmDelete(data: any) {
    this.messageService.clear('d');
    this.serviceContent.myContentDelete(data.contentId, data.languageId).subscribe(
      res => {
        if (res.status === 200) {
          this.list();
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            data,
            detail: this.globalizationMessagesPipe.transform('InfoDeletionOperationSuccessful'),
          });

        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'CD01 ' + res.statusText
          });
          this.ngOnInit();
        }
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.listModel.message = errors.toString();
            this.listModel.hasError = true;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'CD02 ' + this.listModel.message
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'CD03 ' + err.error
            });
          }
        }
        this.ngOnInit();
      }
    );
  }

  onRejectDelete() {
    this.messageService.clear('d');
  }

  clear() {
    this.messageService.clear();
  }

  update(contentId: string, languageId: string) {
    this.router.navigate(['/Content/MyContentUpdate', { contentId, languageId }]);
  }

  newRecordClick() {
    this.router.navigate(['/Content/MyContentAdd']);
  }

}
