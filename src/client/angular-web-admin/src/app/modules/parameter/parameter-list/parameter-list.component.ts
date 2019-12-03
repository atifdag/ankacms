import { Component, OnInit } from '@angular/core';
import { ParameterModel } from 'src/app/models/parameter-model';
import { ListModel } from 'src/app/models/list-model';
import { MenuItem, MessageService, SelectItem } from 'src/app/primeng/components/common/api';
import { ParameterService } from '../parameter.service';
import { Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { ParameterGroupService } from '../../parameter-group/parameter-group.service';

import { FilterModelWithParent } from 'src/app/models/filter-model-with-parent';
import { AppSettingsService } from 'src/app/app-settings.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { IdCodeName } from 'src/app/value-objects/id-code-name';

@Component({
  selector: 'app-parameter-list',
  templateUrl: './parameter-list.component.html',
  styleUrls: ['./parameter-list.component.css']
})
export class ParameterListComponent implements OnInit {

  loading = false;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('ParameterManagement'), routerLink: '/Parameter/List' },
  ];
  parentItems: SelectItem[];
  yearRange: string;
  submitted: boolean;
  statusOptions: any[];
  pageSizes: any[];
  userForm: FormGroup;
  listModel = new ListModel<ParameterModel>();
  filterModel = new FilterModelWithParent();
  selectedPageNumber = 1;
  selectedPageSize = this.appSettingsService.selectedPageSize.key;
  rowsPerPageOptions = this.appSettingsService.rowsPerPageOptions;
  tr = this.appSettingsService.calendarTr;
  tableOps(id: string): MenuItem[] {
    return [
      {
        label: this.globalizationDictionaryPipe.transform('Detail'), icon: 'pi pi-eye', command: () => {
          this.detail(id);
        }
      },
      {
        label: this.globalizationDictionaryPipe.transform('Update'), icon: 'pi pi-pencil', command: () => {
          this.update(id);
        }
      },
      {
        label: this.globalizationDictionaryPipe.transform('Delete'), icon: 'pi pi-trash', command: () => {
          this.showConfirmDelete(id);
        }
      }
    ];
  }

  constructor(
    private messageService: MessageService,
    private appSettingsService: AppSettingsService,
    private serviceParameterGroup: ParameterGroupService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    private serviceParameter: ParameterService,
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
      selectedParentItem: new FormControl()
    });

    this.serviceParameterGroup.keysAndValues().subscribe(
      r => {
        if (r.status === 200) {
          const list = r.body as IdCodeName[];
          if (list.length > 0) {
            this.parentItems = [];
            this.parentItems.push({ label: this.globalizationDictionaryPipe.transform('Select'), value: '' });
            list.forEach(x => {
              this.parentItems.push(
                { value: x.id, label: x.name }
              );
            });
            this.list();
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN01 ' + r.statusText
            });
          }

        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
          });
        }
      },
      err => {
        this.loading = false;
        if (err.status === 400) {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
          });
        }
        setTimeout(() => {
          this.router.navigate(['/Home']);
        }, 1000);
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



    this.serviceParameter.list().subscribe(

      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<ParameterModel>;
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
            this.router.navigate(['/Parameter/List']);
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
    this.serviceParameter.filter(this.filterModel).subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<ParameterModel>;
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
            this.router.navigate(['/Parameter/List']);
          }, 1000);
        }
      }
    );
  }

  detail(id: string) {
    this.router.navigate(['/Parameter/Detail', { id }]);
  }

  showConfirmDelete(id: string) {
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();
    this.messageService.add({
      key: 'd',
      sticky: true,
      data: id,
      severity: 'warn',
      summary: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
      detail: this.globalizationMessagesPipe.transform('QuestionAreYouSureDelete')
    });
  }

  onConfirmDelete(id: string) {
    this.messageService.clear('d');
    this.serviceParameter.delete(id).subscribe(
      res => {
        if (res.status === 200) {
          this.list();
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            data: id,
            detail: this.globalizationMessagesPipe.transform('InfoDeletionOperationSuccessful')
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

  update(id: string) {
    this.router.navigate(['/Parameter/Update', { id }]);
  }

  newRecordClick() {
    this.router.navigate(['/Parameter/Add']);
  }

}
