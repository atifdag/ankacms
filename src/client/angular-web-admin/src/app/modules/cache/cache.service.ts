import { Injectable } from '@angular/core';
import { AppSettingsService } from 'src/app/app-settings.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CacheService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) { }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Cache/List',
      { observe: 'response' }
    );
  }

  delete(key: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Cache/Delete?key=' + key,
      { observe: 'response' }
    );
  }


}
