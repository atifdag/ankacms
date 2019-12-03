import { Component, OnInit } from '@angular/core';
import { UpdateModel } from 'src/app/models/update-model';
import { PermissionModel } from 'src/app/models/permission-model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MessageService } from 'src/app/primeng/components/common/api';
import { PermissionService } from '../permission.service';
import { Router, ActivatedRoute } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { DatePipe } from '@angular/common';
import { IdCodeNameSelected } from 'src/app/value-objects/id-code-name-selected';

@Component({
  selector: 'app-permission-update',
  templateUrl: './permission-update.component.html',
  styleUrls: ['./permission-update.component.css']
})
export class PermissionUpdateComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new UpdateModel<PermissionModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('PermissionManagement'), routerLink: '/Permission/List' },
    { label: this.globalizationDictionaryPipe.transform('Update') },
  ];
  id: string;
  allMenus: IdCodeNameSelected[];
  menus: IdCodeNameSelected[] = [];

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    private servicePermission: PermissionService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    private datePipe: DatePipe,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new PermissionModel();
    this.userForm = new FormGroup({
      code: new FormControl('', Validators.required),
      name: new FormControl('', [Validators.required, Validators.minLength(2)]),
      description: new FormControl(''),
      controllerName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      actionName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      menus: new FormControl(''),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),
      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.servicePermission.beforeUpdate(this.id).subscribe(
        res => {
          this.loading = false;
          if (res.status === 200) {
            this.model = res.body as UpdateModel<PermissionModel>;
            if (this.model.item != null) {

              this.allMenus = this.model.item.menus;
              this.allMenus.forEach(x => {
                if (x.selected) {
                  this.menus.push(x);
                }
              });

              this.userForm.get('code').setValue(this.model.item.code);
              this.userForm.get('name').setValue(this.model.item.name);
              this.userForm.get('description').setValue(this.model.item.description);
              this.userForm.get('controllerName').setValue(this.model.item.controllerName);
              this.userForm.get('actionName').setValue(this.model.item.actionName);
              this.userForm.get('menus').setValue(this.menus);
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
              this.router.navigate(['/Permission/List']);
            }, 1000);
          }
        }
      );
    });
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

  onSubmit() {

    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.adminLayoutComponent.resetCounter();
    if (this.model.item == null) {
      this.model.item = new PermissionModel();
    }
    this.messageService.clear();
    this.model.item.code = this.f.code.value;
    this.model.item.name = this.f.name.value;
    this.model.item.description = this.f.description.value;
    this.model.item.controllerName = this.f.controllerName.value;
    this.model.item.menus = this.f.menus.value;
    this.model.item.actionName = this.f.actionName.value;
    this.model.item.isApproved = this.f.isApproved.value;
    this.servicePermission.update(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            this.router.navigate(['/Permission/List']);
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
    this.router.navigate(['/Permission/List']);
  }

}
