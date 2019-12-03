import { Component, OnInit } from '@angular/core';
import { DetailModel } from 'src/app/models/detail-model';
import { PartModel } from 'src/app/models/part-model';
import { FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { DatePipe } from '@angular/common';
import { MessageService } from 'src/app/primeng/components/common/api';
import { PartService } from '../part.service';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';
import { IdCodeNameSelected } from 'src/app/value-objects/id-code-name-selected';

@Component({
  selector: 'app-part-detail',
  templateUrl: './part-detail.component.html',
  styleUrls: ['./part-detail.component.css']
})
export class PartDetailComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new DetailModel<PartModel>();
  isApprovedChecked: boolean;
  allContents: IdCodeNameSelected[];
  contents: IdCodeNameSelected[] = [];

  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('PartManagement'), routerLink: '/Part/List' },
    { label: this.globalizationDictionaryPipe.transform('Detail') },
  ];
  partId: string;
  languageId: string;

  constructor(
    private route: ActivatedRoute,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private servicePart: PartService,
    private adminLayoutComponent: AdminLayoutComponent,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new PartModel();
    this.userForm = new FormGroup({
      id: new FormControl(''),
      part: new FormControl(''),
      language: new FormControl(''),
      code: new FormControl(''),
      name: new FormControl(''),
      description: new FormControl(''),
      keywords: new FormControl(''),
      contents: new FormControl(''),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),
      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.route.paramMap.subscribe(params => {
      this.partId = params.get('partId');
      this.languageId = params.get('languageId');
    });
    this.servicePart.detail(this.partId, this.languageId).subscribe(
      res => {
        this.loading = false;
        if (res.status === 200) {
          this.model = res.body as DetailModel<PartModel>;
          if (this.model.item != null) {

            this.allContents = this.model.item.contents;
            this.allContents.forEach(x => {
              if (x.selected) {
                this.contents.push(x);
              }
            });
            this.userForm.get('contents').setValue(this.contents);
            this.userForm.get('id').setValue(this.model.item.id);
            this.userForm.get('part').setValue(this.model.item.partId);
            this.userForm.get('language').setValue(this.model.item.language.name);
            this.userForm.get('code').setValue(this.model.item.code);
            this.userForm.get('name').setValue(this.model.item.name);
            this.userForm.get('description').setValue(this.model.item.description);
            this.userForm.get('keywords').setValue(this.model.item.keywords);
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
              this.router.navigate(['/Part/List']);
            }, 1000);
          } else {
            this.model.message = err.error;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN03 ' + this.model.message
            });
            setTimeout(() => {
              this.router.navigate(['/Part/List']);
            }, 3000);
          }
        }
      }
    );
  }

  get f() {
    return this.userForm.controls;
  }

  backClick() {
    this.router.navigate(['/Part/List']);
  }

  updateClick() {
    this.router.navigate(['/Part/Update', { partId: this.partId, languageId: this.languageId }]);
  }

  onConfirmDelete() {
    this.messageService.clear('d');
    this.servicePart.delete(this.partId, this.languageId).subscribe(
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
          this.router.navigate(['/Part/List']);
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
          this.router.navigate(['/Part/List']);
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
