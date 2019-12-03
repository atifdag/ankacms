import { NgModule } from '@angular/core';
import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { AutoCompleteModule } from 'src/app/primeng/components/autocomplete/autocomplete';
import { BreadcrumbModule } from 'src/app/primeng/components/breadcrumb/breadcrumb';
import { ButtonModule } from 'src/app/primeng/components/button/button';
import { CalendarModule } from 'src/app/primeng/components/calendar/calendar';
import { CardModule } from 'src/app/primeng/components/card/card';
import { ChartModule } from 'src/app/primeng/components/chart/chart';
import { CheckboxModule } from 'src/app/primeng/components/checkbox/checkbox';
import { DropdownModule } from 'src/app/primeng/components/dropdown/dropdown';
import { EditorModule } from 'src/app/primeng/components/editor/editor';
import { FieldsetModule } from 'src/app/primeng/components/fieldset/fieldset';
import { InputMaskModule } from 'src/app/primeng/components/inputmask/inputmask';
import { InputTextareaModule } from 'src/app/primeng/components/inputtextarea/inputtextarea';
import { InputTextModule } from 'src/app/primeng/components/inputtext/inputtext';
import { LightboxModule } from 'src/app/primeng/components/lightbox/lightbox';
import { ListboxModule } from 'src/app/primeng/components/listbox/listbox';
import { MessageModule } from 'src/app/primeng/components/message/message';
import { MultiSelectModule } from 'src/app/primeng/components/multiselect/multiselect';
import { OverlayPanelModule } from 'src/app/primeng/components/overlaypanel/overlaypanel';
import { PaginatorModule } from 'src/app/primeng/components/paginator/paginator';
import { PanelModule } from 'src/app/primeng/components/panel/panel';
import { ProgressBarModule } from 'src/app/primeng/components/progressbar/progressbar';
import { ProgressSpinnerModule } from 'src/app/primeng/components/progressspinner/progressspinner';
import { SplitButtonModule } from 'src/app/primeng/components/splitbutton/splitbutton';
import { TableModule } from 'src/app/primeng/components/table/table';
import { ToastModule } from 'src/app/primeng/components/toast/toast';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { FileUploadModule } from 'src/app/primeng/components/fileupload/fileupload';
import { GalleriaModule } from 'src/app/primeng/components/galleria/galleria';
import { DataViewModule } from 'src/app/primeng/components/dataview/dataview';
import { DublinCoreMetadataService } from 'src/app/dublin-core-metadata.service';
import { CarouselModule } from 'src/app/primeng/components/carousel/carousel';


@NgModule({

  declarations: [
    GlobalizationDictionaryPipe,
    GlobalizationMessagesPipe,
  ],

  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    AutoCompleteModule,
    BreadcrumbModule,
    ButtonModule,
    CalendarModule,
    CardModule,
    CarouselModule,
    ChartModule,
    CheckboxModule,
    DataViewModule,
    DropdownModule,
    EditorModule,
    FieldsetModule,
    FileUploadModule,
    GalleriaModule,
    InputMaskModule,
    InputTextareaModule,
    InputTextModule,
    LightboxModule,
    ListboxModule,
    MessageModule,
    MultiSelectModule,
    OverlayPanelModule,
    PaginatorModule,
    PanelModule,
    ProgressBarModule,
    ProgressSpinnerModule,
    SplitButtonModule,
    TableModule,
    ToastModule,
  ],
  exports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    AutoCompleteModule,
    BreadcrumbModule,
    ButtonModule,
    CalendarModule,
    CardModule,
    CarouselModule,
    ChartModule,
    CheckboxModule,
    DataViewModule,
    DropdownModule,
    EditorModule,
    FieldsetModule,
    FileUploadModule,
    GalleriaModule,
    InputMaskModule,
    InputTextareaModule,
    InputTextModule,
    LightboxModule,
    ListboxModule,
    MessageModule,
    MultiSelectModule,
    OverlayPanelModule,
    PaginatorModule,
    PanelModule,
    ProgressBarModule,
    ProgressSpinnerModule,
    SplitButtonModule,
    TableModule,
    ToastModule,
    GlobalizationDictionaryPipe,
    GlobalizationMessagesPipe,
  ],
  providers: [
    DublinCoreMetadataService,
    TitleCasePipe,
    DatePipe,
    GlobalizationDictionaryPipe,
    GlobalizationMessagesPipe
  ]
})
export class SharedModule { }
