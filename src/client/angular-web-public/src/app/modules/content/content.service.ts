import { Injectable } from '@angular/core';
import { AppSettingsService } from 'src/app/app-settings.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) {
  }


  publicDetail(code: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/PublicDetail?code=' + code,
      { observe: 'response' }
    );
  }
}
