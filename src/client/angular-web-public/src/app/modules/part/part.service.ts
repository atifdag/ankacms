import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class PartService {

  constructor(
    private httpClient: HttpClient,
    private appSettingsService: AppSettingsService,
  ) {
  }


  GetPublicPartContents(partCode: string, languageCode: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Part/GetPublicPartContents?partCode=' + partCode + '&languageCode=' + languageCode,
      { observe: 'response' }
    );
  }

}
