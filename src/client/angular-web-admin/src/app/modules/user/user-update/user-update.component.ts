import { Component, OnInit } from '@angular/core';
import { UserModel } from 'src/app/models/user-model';
import { UpdateModel } from 'src/app/models/update-model';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { IdCodeNameSelected } from 'src/app/value-objects/id-code-name-selected';
import { MessageService } from 'src/app/primeng/components/common/api';
import { UserService } from '../user.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-user-update',
  templateUrl: './user-update.component.html',
  styleUrls: ['./user-update.component.css']
})
export class UserUpdateComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new UpdateModel<UserModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('UserManagement'), routerLink: '/User/List' },
    { label: this.globalizationDictionaryPipe.transform('Update') },
  ];
  id: string;
  allRoles: IdCodeNameSelected[];
  roles: IdCodeNameSelected[] = [];

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    private serviceUser: UserService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    private datePipe: DatePipe,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new UserModel();
    this.userForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      username: new FormControl('', [Validators.required, Validators.minLength(8)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      biography: new FormControl(''),
      roles: new FormControl(''),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),
      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.serviceUser.beforeUpdate(this.id).subscribe(
        res => {
          this.loading = false;
          if (res.status === 200) {
            this.model = res.body as UpdateModel<UserModel>;
            if (this.model.item != null) {
              this.allRoles = this.model.item.roles;
              this.allRoles.forEach(x => {
                if (x.selected) {
                  this.roles.push(x);
                }
              });
              this.userForm.get('firstName').setValue(this.model.item.firstName);
              this.userForm.get('lastName').setValue(this.model.item.lastName);
              this.userForm.get('biography').setValue(this.model.item.biography);
              this.userForm.get('username').setValue(this.model.item.username);
              this.userForm.get('email').setValue(this.model.item.email);
              this.userForm.get('roles').setValue(this.roles);
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
              this.router.navigate(['/User/List']);
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
      this.model.item = new UserModel();
    }
    this.messageService.clear();
    this.model.item.firstName = this.f.firstName.value;
    this.model.item.lastName = this.f.lastName.value;
    this.model.item.username = this.f.username.value;
    this.model.item.biography = this.f.biography.value;
    this.model.item.email = this.f.email.value;
    this.model.item.roles = this.f.roles.value;
    this.model.item.isApproved = this.f.isApproved.value;
    this.serviceUser.update(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            this.router.navigate(['/User/List']);
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

            const errors = Object.keys(err.error).map((t)=> {
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
    this.router.navigate(['/User/List']);
  }

}
