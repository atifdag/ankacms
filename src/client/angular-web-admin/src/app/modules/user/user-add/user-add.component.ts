import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl, FormBuilder } from '@angular/forms';
import { AddModel } from 'src/app/models/add-model';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { UserService } from '../user.service';
import { UserModel } from 'src/app/models/user-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-user-add',
  templateUrl: './user-add.component.html',
  styleUrls: ['./user-add.component.css']
})
export class UserAddComponent implements OnInit {
  disabledFieldset: boolean;
  model = new AddModel<UserModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('UserManagement'), routerLink: '/User/List' },
    { label: this.globalizationDictionaryPipe.transform('NewRecord') },
  ];

  constructor(
    private messageService: MessageService,
    private serviceUser: UserService,
    private titlecasePipe: TitleCasePipe,
    private fb: FormBuilder,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    private router: Router
  ) { }

  globalizationMessages(key: string): string {
    return this.globalizationMessagesPipe.transform(key);
  }


  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }

  checkConfirmPassword(group: FormGroup) {
    const password = group.controls.password.value;
    const confirmPassword = group.controls.confirmPassword.value;
    return password === confirmPassword ? null : true;
  }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();

    this.userForm = this.fb.group({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      username: new FormControl('', [Validators.required, Validators.minLength(8)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: [],
      isApproved: new FormControl(true)
    }, {
        validator: this.checkConfirmPassword
      });

    this.model.item = new UserModel();
    this.serviceUser.beforeAdd().subscribe(
      res => {
        if (res.status === 200) {
          this.model = res.body as AddModel<UserModel>;
          if (this.model.item != null) {
            this.userForm.get('firstName').setValue(this.model.item.firstName);
            this.userForm.get('lastName').setValue(this.model.item.lastName);
            this.userForm.get('username').setValue(this.model.item.username);
            this.userForm.get('email').setValue(this.model.item.email);
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
            this.router.navigate(['/User/List']);
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
      this.model.item = new UserModel();
    }
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();
    this.model.item.firstName = this.f.firstName.value;
    this.model.item.lastName = this.f.lastName.value;
    this.model.item.username = this.f.username.value;
    this.model.item.email = this.f.email.value;
    this.model.item.password = this.f.password.value;
    this.model.item.isApproved = this.f.isApproved.value;

    console.log(this.model.item);


    this.serviceUser.add(this.model).subscribe(
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
    this.router.navigate(['/User/List']);
  }


  onBlurFirstName() {
    const firstName = this.f.firstName.value;
    this.userForm.get('firstName').setValue(this.titlecasePipe.transform(firstName));
  }

  onBlurLastName() {
    const firstName = this.f.firstName.value;
    const lastName = this.f.lastName.value;
    this.userForm.get('lastName').setValue(this.upperCase(lastName));
    this.userForm.get('username').setValue(this.convertToSeoLiteral(firstName) + '.' + this.convertToSeoLiteral(lastName));
  }


  convertToSeoLiteral(str: string) {
    str = str.toLowerCase();
    str = str.replace(' ', '.');
    str = str.replace('Ç', 'c');
    str = str.replace('Ğ', 'g');
    str = str.replace('I', 'i');
    str = str.replace('İ', 'i');
    str = str.replace('Ö', 'o');
    str = str.replace('Ş', 's');
    str = str.replace('Ü', 'u');
    str = str.replace('ç', 'c');
    str = str.replace('ğ', 'g');
    str = str.replace('ı', 'i');
    str = str.replace('i', 'i');
    str = str.replace('ö', 'o');
    str = str.replace('ş', 's');
    str = str.replace('ü', 'u');
    str = str.toLowerCase();
    return str;
  }


  upperCase(str: string) {
    str = str.replace('Ç', 'Ç');
    str = str.replace('Ğ', 'Ğ');
    str = str.replace('I', 'I');
    str = str.replace('İ', 'İ');
    str = str.replace('Ö', 'Ö');
    str = str.replace('Ş', 'Ş');
    str = str.replace('Ü', 'Ü');
    str = str.replace('ç', 'Ç');
    str = str.replace('ğ', 'Ğ');
    str = str.replace('ı', 'I');
    str = str.replace('i', 'İ');
    str = str.replace('ö', 'Ö');
    str = str.replace('ş', 'Ş');
    str = str.replace('ü', 'Ü');
    str = str.toUpperCase();
    return str;
  }
}
