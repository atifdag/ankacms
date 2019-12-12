import { Component, OnInit } from '@angular/core';
import { MainService } from './main.service';
import { GlobalizationDictionaryPipe } from './pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from './pipes/globalization-messages.pipe';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    loading: boolean;
    keys: string[] = [];
    cachedValues: string[] = [];
    constructor(
        private titleService: Title,
        private serviceMain: MainService,
        private globalizationDictionaryPipe: GlobalizationDictionaryPipe,
        private globalizationMessagesPipe: GlobalizationMessagesPipe,
    ) { }

    ngOnInit() {
        this.loading = true;
        const title = this.globalizationDictionaryPipe.transform('ApplicationName');
        this.titleService.setTitle(title);
        this.serviceMain.globalizationKeys().subscribe(
            res => {
                if (res.status === 200) {
                    let keys: string[];
                    keys = res.body as string[];
                    keys.forEach(key => {
                        if (localStorage.getItem(key) == null) {
                            if (key.indexOf('glb-dict-') > -1) {
                                this.globalizationDictionaryPipe.transform(key.replace('glb-dict-', ''));
                            } else if (key.indexOf('glb-msg-') > -1) {
                                this.globalizationMessagesPipe.transform(key.replace('glb-msg-', ''));
                            }
                        }
                    });
                    this.loading = false;
                } else {
                }
            },
            err => {
                this.loading = false;
            }
        );
    }
}
