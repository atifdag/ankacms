import { Component, OnInit } from '@angular/core';
import { DetailModel } from 'src/app/models/detail-model';
import { ParameterModel } from 'src/app/models/parameter-model';
import { FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'src/app/primeng/components/common/api';
import { ParameterService } from '../parameter.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { DatePipe } from '@angular/common';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';

@Component({
  selector: 'app-parameter-detail',
  templateUrl: './parameter-detail.component.html',
  styleUrls: ['./parameter-detail.component.css']
})
export class ParameterDetailComponent implements OnInit {
  disabledFieldset: boolean;
  model = new DetailModel<ParameterModel>();
  erasableChecked: boolean;
  isApprovedChecked: boolean;
  loading = true;
  userForm: FormGroup;
  submitted: boolean;
  id: string;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('ParameterManagement'), routerLink: '/Parameter/List' },
    { label: this.globalizationDictionaryPipe.transform('Detail') },
  ];

  constructor(
    private messageService: MessageService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private datePipe: DatePipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private adminLayoutComponent: AdminLayoutComponent,
    private serviceParameter: ParameterService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }


  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new ParameterModel();
    this.userForm = new FormGroup({
      id: new FormControl(''),
      parameterGroup: new FormControl(''),
      code: new FormControl(''),
      key: new FormControl(''),
      value: new FormControl(''),
      description: new FormControl(''),
      erasable: new FormControl(false),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),


      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.serviceParameter.detail(this.id).subscribe(
        res => {
          if (res.status === 200) {
            this.model = res.body as DetailModel<ParameterModel>;
            if (this.model.item != null) {
              this.userForm.get('id').setValue(this.model.item.id);
              this.userForm.get('parameterGroup').setValue(this.model.item.parameterGroup.name);
              this.userForm.get('key').setValue(this.model.item.key);
              this.userForm.get('value').setValue(this.model.item.value);
              this.userForm.get('description').setValue(this.model.item.description);
              this.userForm.get('creator').setValue(this.model.item.creator.name);
              this.userForm.get('creationTime').setValue(
                this.datePipe.transform(this.model.item.creationTime, 'dd/MM/yyyy HH:mm:ss')
              );
              this.userForm.get('lastModifier').setValue(this.model.item.lastModifier.name);
              this.userForm.get('lastModificationTime').setValue(
                this.datePipe.transform(this.model.item.lastModificationTime, 'dd/MM/yyyy HH:mm:ss')
              );
              this.userForm.get('erasable').setValue(this.model.item.erasable);
              this.userForm.get('isApproved').setValue(this.model.item.isApproved);
              this.isApprovedChecked = this.model.item.isApproved;
              this.erasableChecked = this.model.item.erasable;
            }
            this.disabledFieldset = false;
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN01 ' + res.statusText
            });
          }
          this.loading = false;
        },
        err => {
          if (err.status === 400) {
            this.loading = false;
            if (err.error != null) {
              this.model.message = this.globalizationDictionaryPipe.transform('Error');

              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN02 ' + this.model.message
              });
              setTimeout(() => {
                this.router.navigate(['/Parameter/List']);
              }, 1000);
            } else {
              this.model.message = err.error;

              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN03 ' + this.model.message
              });
              setTimeout(() => {
                this.router.navigate(['/Parameter/List']);
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
    this.router.navigate(['/Parameter/List']);
  }

  updateClick() {
    this.router.navigate(['/Parameter/Update', { id: this.id }]);
  }

  onConfirmDelete() {
    this.messageService.clear('d');
    this.serviceParameter.delete(this.id).subscribe(
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
          this.router.navigate(['/Parameter/List']);
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
              detail: 'CD02. ' + this.model.message
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'CD03. ' + err.error
            });
          }
        }
        setTimeout(() => {
          this.router.navigate(['/Parameter/List']);
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
