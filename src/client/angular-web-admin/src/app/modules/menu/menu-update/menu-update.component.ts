import { Component, OnInit } from '@angular/core';
import { UpdateModel } from 'src/app/models/update-model';
import { MenuModel } from 'src/app/models/menu-model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MessageService, SelectItem } from 'src/app/primeng/components/common/api';
import { MenuService } from '../menu.service';
import { Router, ActivatedRoute } from '@angular/router';

import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { DatePipe } from '@angular/common';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { IdCodeName } from 'src/app/value-objects/id-code-name';

@Component({
  selector: 'app-menu-update',
  templateUrl: './menu-update.component.html',
  styleUrls: ['./menu-update.component.css']
})
export class MenuUpdateComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new UpdateModel<MenuModel>();
  isApprovedChecked: boolean;
  parentItems: SelectItem[];
  selectedParentItem: SelectItem = { value: '', label: '' };
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('MenuManagement'), routerLink: '/Menu/List' },
    { label: this.globalizationDictionaryPipe.transform('Update') },
  ];
  id: string;


  constructor(
    private route: ActivatedRoute,
    private adminLayoutComponent: AdminLayoutComponent,
    private messageService: MessageService,
    private serviceMenu: MenuService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private datePipe: DatePipe,
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
    this.model.item = new MenuModel();
    this.userForm = new FormGroup({
      selectedParentItem: new FormControl('', Validators.required),
      code: new FormControl('', [Validators.required, Validators.minLength(2)]),
      name: new FormControl('', [Validators.required, Validators.minLength(2)]),
      description: new FormControl(''),
      address: new FormControl(''),
      icon: new FormControl(''),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),

      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.serviceMenu.keysAndValues().subscribe(
      r => {
        if (r.status === 200) {
          const list = r.body as IdCodeName[];
          if (list.length > 0) {
            this.parentItems = [];
            list.forEach(x => {
              this.parentItems.push(
                { value: x.id, label: x.name }
              );
            });
            this.parentItems.push({ label: this.globalizationDictionaryPipe.transform('Select'), value: '' });
            this.route.paramMap.subscribe(params => {
              this.id = params.get('id');
              this.serviceMenu.beforeUpdate(this.id).subscribe(
                res => {
                  this.loading = false;
                  if (res.status === 200) {
                    this.model = res.body as UpdateModel<MenuModel>;
                    if (this.model.item != null) {

                      const selected = this.parentItems.find(x => x.value === this.model.item.parentMenu.id);

                      if (selected !== undefined) {
                        this.selectedParentItem = selected;
                      }
                      this.userForm.get('selectedParentItem').setValue(this.selectedParentItem.value);
                      this.userForm.get('code').setValue(this.model.item.code);
                      this.userForm.get('name').setValue(this.model.item.name);
                      this.userForm.get('description').setValue(this.model.item.description);
                      this.userForm.get('address').setValue(this.model.item.address);
                      this.userForm.get('icon').setValue(this.model.item.icon);
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
                      this.model.message = 'Hata oluştu!';
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
                      this.router.navigate(['/Menu/List']);
                    }, 1000);
                  }
                }
              );
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: this.globalizationMessagesPipe.transform('DangerParentNotFound')
            });
            setTimeout(() => {
              this.router.navigate(['/Menu/List']);
            }, 1000);
          }
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'IN05 ' + r.statusText
          });
        }
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.model.message = this.globalizationDictionaryPipe.transform('Error');
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN06 ' + this.model.message
            });
          } else {
            this.model.message = err.error;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN07 ' + this.model.message
            });
          }
          setTimeout(() => {
            this.router.navigate(['/Menu/List']);
          }, 1000);
        }
      }
    );
  }

  get f() {
    return this.userForm.controls;
  }

  onSubmit() {

    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();

    if (this.model.item == null) {
      this.model.item = new MenuModel();
    }
    if (this.f.selectedParentItem.value.value === '') {
      this.model.item.parentMenu.id = this.selectedParentItem.value;
    } else {
      this.model.item.parentMenu.id = this.f.selectedParentItem.value;
    }
    this.model.item.code = this.f.code.value;
    this.model.item.name = this.f.name.value;
    this.model.item.description = this.f.description.value;
    this.model.item.address = this.f.address.value;
    this.model.item.icon = this.f.icon.value;
    this.model.item.isApproved = this.f.isApproved.value;

    this.serviceMenu.update(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            this.router.navigate(['/Menu/List']);
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
        console.log(err);
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
    this.router.navigate(['/Menu/List']);
  }

}
