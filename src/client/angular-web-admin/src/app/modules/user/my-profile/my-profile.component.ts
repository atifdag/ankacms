import { Component, OnInit } from '@angular/core';
import { MyProfileModel } from 'src/app/models/my-profile-model';
import { UserService } from '../user.service';
import { MessageService } from 'src/app/primeng/components/common/messageservice';
import { AuthenticationService } from '../../authentication/authentication.service';
import { Router } from '@angular/router';
import { SignOutOption } from 'src/app/value-objects/sign-out-option.enum';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';


@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css'],
  styles: [`
        :host ::ng-deep button {
            margin-right: .5em;
        }
    `]
})
export class MyProfileComponent implements OnInit {

  profileModel = new MyProfileModel();

  constructor(
    private router: Router,
    private userService: UserService,
    private messageService: MessageService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private authenticationService: AuthenticationService,
  ) { }

  ngOnInit() {


    if (sessionStorage.getItem('myProfile')) {
      const jsonObj: MyProfileModel = JSON.parse(
        sessionStorage.getItem('myProfile')

      );
      this.profileModel = jsonObj as MyProfileModel;
    } else {
      this.userService.myProfile().subscribe(
        res => {
          if (res.status === 200) {
            this.profileModel = res.body as MyProfileModel;
            sessionStorage.setItem('myProfile', JSON.stringify(this.profileModel));
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
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN02 ' + err.error.message
              });
            } else {
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN03 ' + err
              });

            }
          }
        }

      );
    }
  }

  showConfirm() {
    this.messageService.clear();
    this.messageService.add({
      key: 'c',
      sticky: true,
      severity: 'warn',
      summary: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
      detail: this.globalizationMessagesPipe.transform('QuestionAreYouSureLogout')
    });
  }

  onConfirm() {
    this.messageService.clear('c');
    this.signOut();
  }

  onReject() {
    this.messageService.clear('c');
  }

  clear() {
    this.messageService.clear();
  }

  signOut() {
    this.authenticationService.signOut(SignOutOption.ValidLogout).subscribe();
    this.messageService.clear();
    this.messageService.add({
      severity: 'success',
      summary: this.globalizationDictionaryPipe.transform('Success'),
      detail: this.globalizationMessagesPipe.transform('InfoLogoutOperationSuccessful')
        + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle')
    });
    setTimeout(() => {
      this.router.navigate(['/Home']);
    }, 1000);
  }


}
