import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { AuthenticationService } from '../authentication.service';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {

  disabledFieldset: boolean;
  userForm: FormGroup;
  submitted: boolean;
  loading = false;
  constructor(
    private fb: FormBuilder,
    private authenticationService: AuthenticationService,
    private router: Router,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.userForm = this.fb.group({
      username: new FormControl('', Validators.required)
    });
    this.disabledFieldset = false;
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
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
    const username = this.f.username.value;
    this.authenticationService.forgotPassword(username).subscribe(
      res => {
        if (res.status === 200) {
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı!',
            detail: 'Şifre başarıyla gönderildi. E-posta gelen kutunuzu kontrol ediniz. Şimdi Yönlendiriliyorsunuz...'
          });
          setTimeout(() => {
            this.router.navigate(['Authentication/SignIn']);
          }, 3000);
        } else {
          this.loading = false;
          this.messageService.add({ severity: 'error', summary: 'Hata!1', detail: 'Kod: AUFP001 ' + res.statusText });
          console.log(res);
        }
      },
      err => {
        this.loading = false;
        this.messageService.add({ severity: 'error', summary: 'Hata!', detail: err.error });
        console.log(err);
      }
    );

  }

}
