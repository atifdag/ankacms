import { Injectable } from '@angular/core';
import { HttpResponse, HttpClient } from '@angular/common/http';
import { AppSettingsService } from 'src/app/app-settings.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) {
  }


  publicDetail(code: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Category/PublicDetail?code=' + code,
      { observe: 'response' }
    );
  }
}
