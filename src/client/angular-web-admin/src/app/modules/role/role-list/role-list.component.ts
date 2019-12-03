import { Component, OnInit } from '@angular/core';
import { RoleModel } from 'src/app/models/role-model';
import { ListModel } from 'src/app/models/list-model';
import { MenuItem, MessageService } from 'src/app/primeng/components/common/api';
import { RoleService } from '../role.service';
import { Router } from '@angular/router';
import { FilterModel } from 'src/app/models/filter-model';
import { FormGroup, FormControl } from '@angular/forms';
import { AppSettingsService } from 'src/app/app-settings.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})
export class RoleListComponent implements OnInit {

  loading = true;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('RoleManagement'), routerLink: '/Role/List' },
  ];

  yearRange: string;
  submitted: boolean;
  statusOptions: any[];
  pageSizes: any[];
  userForm: FormGroup;
  listModel = new ListModel<RoleModel>();
  filterModel = new FilterModel();
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
    private serviceRole: RoleService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
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

  paginate(event) {
    this.selectedPageNumber = event.page + 1;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.selectedPageSize = event.rows;
    this.filterModel.pageSize = this.selectedPageSize;
    this.changeForm();
  }

  list() {
    this.serviceRole.list().subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<RoleModel>;
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
            this.listModel.message = 'Hata oluştu!';
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
            this.router.navigate(['/Role/List']);
          }, 1000);
        }
      }
    );
  }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.userForm = new FormGroup({
      searched: new FormControl(this.filterModel.searched),
      startDate: new FormControl(this.filterModel.startDate),
      endDate: new FormControl(this.filterModel.endDate),
      selectedStatus: new FormControl(this.appSettingsService.selectedStatus),
    });
    this.list();
  }

  changeForm() {
    this.loading = true;
    this.submitted = true;
    this.messageService.clear();
    this.filterModel.searched = this.f.searched.value;
    this.filterModel.startDate = this.f.startDate.value;
    this.filterModel.endDate = this.f.endDate.value;
    this.filterModel.status = this.f.selectedStatus.value.key;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;
    this.filter();
  }

  get f() {
    return this.userForm.controls;
  }

  filter() {
    this.adminLayoutComponent.resetCounter();
    this.serviceRole.filter(this.filterModel).subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<RoleModel>;
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
            this.router.navigate(['/Role/List']);
          }, 1000);
        }
      }
    );
  }

  detail(id: string) {
    this.router.navigate(['/Role/Detail', { id: id }]);
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
    this.serviceRole.delete(id).subscribe(
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
    this.router.navigate(['/Role/Update', { id: id }]);
  }

  newRecordClick(event: Event) {
    this.router.navigate(['/Role/Add']);
  }
}
