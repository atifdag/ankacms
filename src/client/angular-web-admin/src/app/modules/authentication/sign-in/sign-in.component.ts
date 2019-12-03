import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { AuthenticationService } from '../authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/messageservice';
import { SignInModel } from 'src/app/models/sign-in-model';
import { IdentityService } from 'src/app/identity.service';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { MainService } from 'src/app/main.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css'],
})
export class SignInComponent implements OnInit {
  username: string;
  userForm: FormGroup;
  submitted: boolean;
  loading: boolean;
  disabledFieldset: boolean;
  model: SignInModel;
  errorMessage: string;
  redirectUrl: string;
  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private serviceAuthentication: AuthenticationService,
    private serviceMain: MainService,
    private router: Router,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private identityService: IdentityService,
    private messageService: MessageService
  ) {

  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }

  ngOnInit() {
    this.loading = true;
    this.disabledFieldset = false;
    this.userForm = this.fb.group({
      username: new FormControl('', [Validators.required, Validators.minLength(8)]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });


    this.serviceMain.test().subscribe(
      res => {
        if (res.status === 200) {
          this.loading = false;
          this.disabledFieldset = false;
          this.route.paramMap.subscribe(params => {
            this.redirectUrl = params.get('redirectUrl');
          });

        } else {
          this.loading = false;
          this.disabledFieldset = true;
          this.messageService.add(
            {
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN01 ' + res.body
            });
        }
      },
      err => {
        this.disabledFieldset = true;
        this.loading = false;
        this.messageService.add(
          {
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'IN02. Api servisi yanıt vermiyor2!'
          });
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
    this.messageService.clear();
    this.loading = true;
    this.model = new SignInModel();
    this.model.username = this.f.username.value;
    this.model.password = this.f.password.value;
    this.serviceAuthentication.signIn(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.identityService.set(res.body);
          this.disabledFieldset = true;
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoLoginOperationSuccessful')
              + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle')
          });
          let url: any[];
          if (this.redirectUrl != null) {
            if (this.redirectUrl.indexOf(';id=') > 0) {
              const arrUrl = this.redirectUrl.split(';id=');
              url = [arrUrl[0], { id: arrUrl[1] }];
            } else {
              url = [this.redirectUrl];
            }
          } else {
            url = ['/Dashboard/Index'];
          }
          setTimeout(() => {
            this.router.navigate(url);
          }, 1000);
        } else if (res.status === 401) {
          this.loading = false;
          this.messageService.add(
            {
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: res.body
            });
        } else if (res.status === 403) {
          this.loading = false;
          this.messageService.add(
            {
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'SU01 ' + res.body
            });
        } else {
          this.loading = false;
          this.messageService.add(
            {
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'SU02 ' + res.body
            });
        }
      },
      err => {
        this.loading = false;
        this.messageService.add(
          {
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'SU04 ' + err.error
          });
      }
    );

  }
}
