import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { AddModel } from 'src/app/models/add-model';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { ParameterGroupService } from '../parameter-group.service';
import { ParameterGroupModel } from 'src/app/models/parameter-group-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';

@Component({
  selector: 'app-parameter-group-add',
  templateUrl: './parameter-group-add.component.html',
  styleUrls: ['./parameter-group-add.component.css']
})
export class ParameterGroupAddComponent implements OnInit {
  disabledFieldset: boolean;
  model = new AddModel<ParameterGroupModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('ParameterGroupManagement'), routerLink: '/ParameterGroup/List' },
    { label: this.globalizationDictionaryPipe.transform('NewRecord') },
  ];

  constructor(
    private messageService: MessageService,
    private serviceParameterGroup: ParameterGroupService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
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
    this.userForm = new FormGroup({
      code: new FormControl('', Validators.required),
      name: new FormControl('', [Validators.required, Validators.minLength(2)]),
      description: new FormControl(''),
      isApproved: new FormControl(true)
    });
    this.model.item = new ParameterGroupModel();
    this.serviceParameterGroup.beforeAdd().subscribe(
      res => {
        if (res.status === 200) {
          this.model = res.body as AddModel<ParameterGroupModel>;
          if (this.model.item != null) {
            this.userForm.get('code').setValue(this.model.item.code);
            this.userForm.get('name').setValue(this.model.item.name);
            this.userForm.get('description').setValue(this.model.item.description);
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
            this.router.navigate(['/ParameterGroup/List']);
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

    if (this.model.item == null) {
      this.model.item = new ParameterGroupModel();
    }
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();
    this.model.item.code = this.f.code.value;
    this.model.item.name = this.f.name.value;
    this.model.item.description = this.f.description.value;
    this.model.item.isApproved = this.f.isApproved.value;

    console.log(this.model.item);


    this.serviceParameterGroup.add(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            this.router.navigate(['/ParameterGroup/List']);
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
    this.router.navigate(['/ParameterGroup/List']);
  }
}
