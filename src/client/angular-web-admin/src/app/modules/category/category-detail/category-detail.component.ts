import { Component, OnInit } from '@angular/core';
import { DetailModel } from 'src/app/models/detail-model';
import { CategoryModel } from 'src/app/models/category-model';
import { FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { DatePipe } from '@angular/common';
import { MessageService } from 'src/app/primeng/components/common/api';
import { CategoryService } from '../category.service';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';

@Component({
  selector: 'app-category-detail',
  templateUrl: './category-detail.component.html',
  styleUrls: ['./category-detail.component.css']
})
export class CategoryDetailComponent implements OnInit {
  loading = true;
  disabledFieldset: boolean;
  model = new DetailModel<CategoryModel>();
  isApprovedChecked: boolean;
  userForm: FormGroup;
  submitted: boolean;
  breadcrumbItems = [
    { label: this.globalizationDictionaryPipe.transform('HomePage'), routerLink: '/Home' },
    { label: this.globalizationDictionaryPipe.transform('CategoryManagement'), routerLink: '/Category/List' },
    { label: this.globalizationDictionaryPipe.transform('Detail') },
  ];
  categoryId: string;
  languageId: string;

  constructor(
    private route: ActivatedRoute,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private serviceCategory: CategoryService,
    private adminLayoutComponent: AdminLayoutComponent,
    private router: Router
  ) { }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.model.item = new CategoryModel();
    this.userForm = new FormGroup({
      id: new FormControl(''),
      category: new FormControl(''),
      language: new FormControl(''),
      code: new FormControl(''),
      name: new FormControl(''),
      description: new FormControl(''),
      keywords: new FormControl(''),
      creator: new FormControl(''),
      creationTime: new FormControl(''),
      lastModifier: new FormControl(''),
      lastModificationTime: new FormControl(''),
      isApproved: new FormControl('')
    });

    this.disabledFieldset = false;
    this.route.paramMap.subscribe(params => {
      this.categoryId = params.get('categoryId');
      this.languageId = params.get('languageId');
    });
    this.serviceCategory.detail(this.categoryId, this.languageId).subscribe(
      res => {
        this.loading = false;
        if (res.status === 200) {
          this.model = res.body as DetailModel<CategoryModel>;
          if (this.model.item != null) {
            this.userForm.get('id').setValue(this.model.item.id);
            this.userForm.get('category').setValue(this.model.item.categoryId);
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
              this.router.navigate(['/Category/List']);
            }, 1000);
          } else {
            this.model.message = err.error;
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN03 ' + this.model.message
            });
            setTimeout(() => {
              this.router.navigate(['/Category/List']);
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
    this.router.navigate(['/Category/List']);
  }

  updateClick() {
    this.router.navigate(['/Category/Update', { categoryId: this.categoryId, languageId: this.languageId }]);
  }

  onConfirmDelete() {
    this.messageService.clear('d');
    this.serviceCategory.delete(this.categoryId, this.languageId).subscribe(
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
          this.router.navigate(['/Category/List']);
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
          this.router.navigate(['/Category/List']);
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
