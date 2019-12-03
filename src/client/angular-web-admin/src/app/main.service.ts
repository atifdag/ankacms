import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MainService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient
  ) { }

  test(): Observable<HttpResponse<any>> {
    return this.httpClient.get(this.appSettingsService.apiUrl + '/Home', { observe: 'response' });
  }

  globalizationKeys(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Home/GlobalizationKeys',
      { observe: 'response' }
    );
  }
}
