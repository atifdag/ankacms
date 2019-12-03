import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { UserService } from '../user.service';
import { UpdatePasswordModel } from 'src/app/models/update-password-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';

@Component({
  selector: 'app-update-my-password',
  templateUrl: './update-my-password.component.html',
  styleUrls: ['./update-my-password.component.css']
})
export class UpdateMyPasswordComponent implements OnInit {
  disabledFieldset: boolean;
  userForm: FormGroup;
  submitted: boolean;
  loading = false;
  model: UpdatePasswordModel;

  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('UserProfile'), routerLink: '/User/MyProfile' },
    { label: this.globalizationDictionaryPipe.transform('UpdateMyPassword') },
  ];
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private messageService: MessageService
  ) { }


  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }

  ngOnInit() {
    this.userForm = this.fb.group({
      oldPassword: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
      confirmPassword: []
    }, {
        validator: this.checkConfirmPassword
      });
    this.disabledFieldset = false;
  }

  get f() {
    return this.userForm.controls;
  }


  checkConfirmPassword(group: FormGroup) {
    const password = group.controls.password.value;
    const confirmPassword = group.controls.confirmPassword.value;
    return password === confirmPassword ? null : true;
  }

  onSubmit() {
    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.messageService.clear();
    this.loading = true;
    this.model = new UpdatePasswordModel();
    this.model.oldPassword = this.f.oldPassword.value;
    this.model.password = this.f.password.value;
    this.model.confirmPassword = this.f.confirmPassword.value;
    this.userService.updateMyPassword(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
          });
          setTimeout(() => {
            sessionStorage.removeItem('myProfile');
            this.router.navigate(['/User/MyProfile']);
          }, 1000);
        } else {
          this.loading = false;
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
            this.loading = false;
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
            this.loading = false;
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
    this.router.navigate(['/User/MyProfile']);
  }

}
