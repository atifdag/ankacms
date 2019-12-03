import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { UpdateMyInformationModel } from 'src/app/models/update-my-information-model';
import { MyProfileModel } from 'src/app/models/my-profile-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-update-my-information',
  templateUrl: './update-my-information.component.html',
  styleUrls: ['./update-my-information.component.css']
})
export class UpdateMyInformationComponent implements OnInit {

  disabledFieldset: boolean;
  userForm: FormGroup;
  submitted: boolean;
  loading = false;
  model: UpdateMyInformationModel;
  profileModel = new MyProfileModel();

  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('UserProfile'), routerLink: '/User/MyProfile' },
    { label: this.globalizationDictionaryPipe.transform('UpdateMyInformation') },
  ];

  constructor(
    private userService: UserService,
    private router: Router,
    private datePipe: DatePipe,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private messageService: MessageService
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

  ngOnInit() {


    const jsonObj: MyProfileModel = JSON.parse(
      sessionStorage.getItem('myProfile')
    );
    this.profileModel = jsonObj as MyProfileModel;
    this.userForm = new FormGroup({
      id: new FormControl({ value: this.profileModel.userModel.id, disabled: true }, Validators.required),
      username: new FormControl(this.profileModel.userModel.username, [Validators.required, Validators.minLength(5)]),
      email: new FormControl(this.profileModel.userModel.email, [Validators.required, Validators.email]),
      firstName: new FormControl(this.profileModel.userModel.firstName, [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl(this.profileModel.userModel.lastName, [Validators.required, Validators.minLength(2)]),
      biography: new FormControl(),
      creator: new FormControl(this.profileModel.userModel.creator.name),
      creationTime: new FormControl(
        this.datePipe.transform(this.profileModel.userModel.creationTime, 'dd/MM/yyyy HH:mm:ss')
      ),
      lastModifier: new FormControl(this.profileModel.userModel.lastModifier.name),
      lastModificationTime: new FormControl(
        this.datePipe.transform(this.profileModel.userModel.lastModificationTime, 'dd/MM/yyyy HH:mm:ss')
      ),
    });
    this.disabledFieldset = false;
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
    this.model = new UpdateMyInformationModel();
    this.model.username = this.f.username.value;
    this.model.email = this.f.email.value;
    this.model.firstName = this.f.firstName.value;
    this.model.lastName = this.f.lastName.value;
    this.model.biography = this.f.biography.value;

    this.userService.updateMyInformation(this.model).subscribe(
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
