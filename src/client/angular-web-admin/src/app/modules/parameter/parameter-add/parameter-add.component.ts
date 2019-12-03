import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { AddModel } from 'src/app/models/add-model';
import { Router } from '@angular/router';
import { MessageService, SelectItem } from 'src/app/primeng/components/common/api';
import { ParameterService } from '../parameter.service';
import { ParameterModel } from 'src/app/models/parameter-model';
import { ParameterGroupService } from '../../parameter-group/parameter-group.service';

import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { IdCodeName } from 'src/app/value-objects/id-code-name';
 
@Component({
  selector: 'app-parameter-add',
  templateUrl: './parameter-add.component.html',
  styleUrls: ['./parameter-add.component.css']
})
export class ParameterAddComponent implements OnInit {
  disabledFieldset: boolean;
  model = new AddModel<ParameterModel>();
  userForm: FormGroup;
  parentItems: SelectItem[];
  erasableChecked: boolean;
  isApprovedChecked: boolean;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('ParameterManagement'), routerLink: '/Parameter/List' },
    { label: this.globalizationDictionaryPipe.transform('NewRecord') },
  ];

  constructor(
    private messageService: MessageService,
    private serviceParameterGroup: ParameterGroupService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceParameter: ParameterService,
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
    this.model.item = new ParameterModel();

    this.userForm = new FormGroup({
      selectedParentItem: new FormControl('', Validators.required),
      key: new FormControl('', [Validators.required, Validators.minLength(2)]),
      value: new FormControl('', [Validators.required, Validators.minLength(2)]),
      description: new FormControl(''),
      erasable: new FormControl(true),
      isApproved: new FormControl(true)
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

            this.serviceParameter.beforeAdd().subscribe(
              res => {
                if (res.status === 200) {
                  this.model = res.body as AddModel<ParameterModel>;
                  if (this.model.item != null) {
                    this.userForm.get('key').setValue(this.model.item.key);
                    this.userForm.get('value').setValue(this.model.item.value);
                    this.userForm.get('description').setValue(this.model.item.description);
                    this.userForm.get('erasable').setValue(this.model.item.erasable);
                    this.userForm.get('isApproved').setValue(this.model.item.isApproved);
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
                    this.router.navigate(['/Parameter/List']);
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
              this.router.navigate(['/Parameter/List']);
            }, 1000);
          }
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'LI05 ' + r.statusText
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
              detail: 'LI06 ' + this.model.message
            });
          } else {
            this.model.message = err.error;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'LI07 ' + this.model.message
            });
          }
          setTimeout(() => {
            this.router.navigate(['/Parameter/List']);
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
      this.model.item = new ParameterModel();
    }
    if (this.model.item.parameterGroup == null) {
      this.model.item.parameterGroup = new IdCodeName();
    }
    this.model.item.parameterGroup.id = this.f.selectedParentItem.value;
    this.model.item.key = this.f.key.value;
    this.model.item.value = this.f.value.value;
    this.model.item.description = this.f.description.value;
    this.model.item.erasable = this.f.erasable.value;
    this.model.item.isApproved = this.f.isApproved.value;
    this.serviceParameter.add(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            this.router.navigate(['/Parameter/List']);
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
    this.router.navigate(['/Parameter/List']);
  }
}
