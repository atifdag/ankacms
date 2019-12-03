import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PartModel } from 'src/app/models/part-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AddModel } from 'src/app/models/add-model';
import { AppSettingsService } from 'src/app/app-settings.service';
import { FilterModelWithLanguage } from 'src/app/models/filter-model-with-language';

@Injectable({
  providedIn: 'root'
})
export class PartService {

  constructor(
    private httpClient: HttpClient,
    private appSettingsService: AppSettingsService,
  ) {
  }


  GetPublicCarouselContents(partCode: string, languageCode: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Part/GetPublicCarouselContents?partCode=' + partCode + '&languageCode=' + languageCode,
      { observe: 'response' }
    );
  }


  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Part/List', { observe: 'response' }
    );
  }

  filter(model: FilterModelWithLanguage): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Part/Filter',
      model,
      { observe: 'response' });
  }

  detail(partId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Part/Detail?partId=' + partId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Part/Add',
      { observe: 'response' }
    );
  }

  add(model: AddModel<PartModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Part/Add',
      model,
      { observe: 'response' });
  }

  beforeUpdate(partId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Part/Update?partId=' + partId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<PartModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Part/Update',
      model,
      { observe: 'response' });
  }

  delete(partId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Part/Delete?partId=' + partId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

}
