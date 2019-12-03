import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { AuthenticationService } from '../authentication.service';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { SignUpModel } from 'src/app/models/sign-up-model';
import { TitleCasePipe } from '@angular/common';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {

  userForm: FormGroup;
  submitted: boolean;
  loading = false;
  disabledFieldset: boolean;
  model: SignUpModel;

  constructor(
    private titlecasePipe: TitleCasePipe,
    private fb: FormBuilder,
    private authenticationService: AuthenticationService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private router: Router,
    private messageService: MessageService
  ) { }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessages(key: string): string {
    return this.globalizationMessagesPipe.transform(key);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }



  ngOnInit() {
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
    this.disabledFieldset = false;
  }

  checkConfirmPassword(group: FormGroup) {
    const password = group.controls.password.value;
    const confirmPassword = group.controls.confirmPassword.value;
    return password === confirmPassword ? null : true;
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

  get f() {
    return this.userForm.controls;
  }

  onSubmit() {
    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.messageService.clear();
    this.loading = true;
    this.model = new SignUpModel();
    this.model.firstName = this.f.firstName.value;
    this.model.lastName = this.f.lastName.value;
    this.model.username = this.f.username.value;
    this.model.email = this.f.email.value;
    this.model.password = this.f.password.value;
    this.model.confirmPassword = this.f.confirmPassword.value;
    this.authenticationService.signUp(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı!',
            detail: 'Kaydetme işlemi başarıyla tamamlandı. E-posta gelen kutunuzu kontrol ediniz. Şimdi Yönlendiriliyorsunuz...'
          });
          setTimeout(() => {
            this.router.navigate(['/Authentication/SignIn']);
          }, 3000);
         // console.log(res);
        } else {
          this.loading = false;
          this.messageService.add({ severity: 'error', summary: 'Hata!', detail: 'Kod: AUSU001 ' + res.statusText });
          console.log(res);
        }
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.loading = false;
            this.model = err.error;
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t] + ' ';
            });
            this.messageService.add({ severity: 'error', summary: 'Hata!', detail: 'Kod: AUSU002 ' + errors.toString() });
          } else {
            this.loading = false;
            this.messageService.add({ severity: 'error', summary: 'Hata!', detail: 'Kod: AUSU003 ' + err.error });
          }
        }
      }
    );

  }

}
