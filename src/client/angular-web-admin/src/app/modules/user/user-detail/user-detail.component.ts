import { Component, OnInit } from '@angular/core';
import { DetailModel } from 'src/app/models/detail-model';
import { UserModel } from 'src/app/models/user-model';
import { UserService } from '../user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/messageservice';
import { FormGroup, FormControl } from '@angular/forms';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { DatePipe } from '@angular/common';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { IdCodeNameSelected } from 'src/app/value-objects/id-code-name-selected';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new DetailModel<UserModel>();
  isApprovedChecked: boolean;

  allRoles: IdCodeNameSelected[];
  roles: IdCodeNameSelected[] = [];
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('UserManagement'), routerLink: '/User/List' },
    { label: this.globalizationDictionaryPipe.transform('Detail') },
  ];
  id: string;

  constructor(
    private route: ActivatedRoute,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private serviceUser: UserService,
    private adminLayoutComponent: AdminLayoutComponent,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new UserModel();
    this.userForm = new FormGroup({
      id: new FormControl(''),
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      biography: new FormControl(''),
      username: new FormControl(''),
      email: new FormControl(''),
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
      this.serviceUser.detail(this.id).subscribe(
        res => {
          this.loading = false;
          if (res.status === 200) {
            this.model = res.body as DetailModel<UserModel>;
            if (this.model.item != null) {
              this.allRoles = this.model.item.roles;
              this.allRoles.forEach(x => {
                if (x.selected) {
                  this.roles.push(x);
                }
              });
              this.userForm.get('id').setValue(this.model.item.id);
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
              setTimeout(() => {
                this.router.navigate(['/User/List']);
              }, 1000);
            } else {
              this.model.message = err.error;
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN03 ' + this.model.message
              });
              setTimeout(() => {
                this.router.navigate(['/User/List']);
              }, 3000);
            }
          }
        }
      );
    });
  }

  get f() {
    return this.userForm.controls;
  }

  backClick() {
    this.router.navigate(['/User/List']);
  }

  updateClick() {
    this.router.navigate(['/User/Update', { id: this.id }]);
  }

  onConfirmDelete() {
    this.messageService.clear('d');
    this.serviceUser.delete(this.id).subscribe(
      res => {
        if (res.status === 200) {
          this.messageService.add({
            severity: 'success',
            summary: this.globalizationDictionaryPipe.transform('Success'),
            detail: this.globalizationMessagesPipe.transform('InfoDeletionOperationSuccessful')
          });
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'CD01. ' + res.statusText
          });
        }
        setTimeout(() => {
          this.router.navigate(['/User/List']);
        }, 3000);
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.model.message = errors.toString();
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'Kod: CD02. ' + this.model.message
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'Kod: CD03. ' + err.error
            });
          }
        }
        setTimeout(() => {
          this.router.navigate(['/User/List']);
        }, 3000);
      }
    );
  }

  showConfirmDelete() {
    this.adminLayoutComponent.resetCounter();
    this.messageService.clear();
    this.messageService.add({
      key: 'd',
      sticky: true,
      severity: 'warn',
      summary: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
      detail: this.globalizationMessagesPipe.transform('QuestionAreYouSureDelete')
    });
  }


  onRejectDelete() {
    this.messageService.clear('d');
  }

  clear() {
    this.messageService.clear();
  }
}
