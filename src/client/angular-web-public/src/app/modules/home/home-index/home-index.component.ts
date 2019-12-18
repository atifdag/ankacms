import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { PublicPartModel } from 'src/app/models/public-part-model';
import { MainService } from 'src/app/main.service';
import { MessageService } from 'src/app/primeng/components/common/api';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AppSettingsService } from 'src/app/app-settings.service';
import { Router } from '@angular/router';
import { PartService } from '../../part/part.service';
import { PublicContentModel } from 'src/app/models/public-content-model';

@Component({
  selector: 'app-home-index',
  templateUrl: './home-index.component.html',
  styleUrls: ['./home-index.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class HomeIndexComponent implements OnInit {

  loading = true;
  loadingCarouselModel = true;
  carouselModel = new PublicPartModel();
  homeCardModel = new PublicPartModel();
  highlightsModel = new PublicPartModel();
  responsiveOptions = [
    {
      breakpoint: '1024px',
      numVisible: 3,
      numScroll: 3
    },
    {
      breakpoint: '768px',
      numVisible: 2,
      numScroll: 2
    },
    {
      breakpoint: '560px',
      numVisible: 1,
      numScroll: 1
    }
  ];

  constructor(
    private servicePart: PartService,
    private serviceMain: MainService,
    private messageService: MessageService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    public appSettingsService: AppSettingsService,
    private router: Router
  ) {
    this.carouselModel.contents = [
      new PublicContentModel()
    ];
  }

  ngOnInit() {
    this.fillCarouselModel();
    this.fillCardModel();
    this.fillHighlightsModel();
  }

  fillCarouselModel() {
    this.servicePart.GetPublicPartContents('MANSET', 'tr').subscribe(
      res => {
        if (res.status === 200) {
          this.carouselModel = res.body as PublicPartModel;
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'Hata oluştu.'
          });
        }
        this.loadingCarouselModel = false;
      },
    );
  }

  fillCardModel() {
    this.servicePart.GetPublicPartContents('MANSETALTI', 'tr').subscribe(
      res => {
        if (res.status === 200) {
          this.homeCardModel = res.body as PublicPartModel;
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'Hata oluştu.'
          });
        }
        this.loadingCarouselModel = false;
      },
    );
  }

  fillHighlightsModel() {
    this.servicePart.GetPublicPartContents('ONECIKANLAR', 'tr').subscribe(
      res => {
        if (res.status === 200) {
          this.highlightsModel = res.body as PublicPartModel;
        } else {
          this.messageService.add({
            severity: 'error',
            summary: this.globalizationDictionaryPipe.transform('Error'),
            detail: 'Hata oluştu.'
          });
        }
        this.loadingCarouselModel = false;
      },
    );
  }

}
