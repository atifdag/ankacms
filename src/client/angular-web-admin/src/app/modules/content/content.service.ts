import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContentModel } from 'src/app/models/content-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AddModel } from 'src/app/models/add-model';
import { AppSettingsService } from 'src/app/app-settings.service';
import { FilterModelWithLanguage } from 'src/app/models/filter-model-with-language';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  constructor(
    private httpClient: HttpClient,
    private appSettingsService: AppSettingsService,
  ) {
  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/List', { observe: 'response' }
    );
  }

  myContentList(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/MyContentList', { observe: 'response' }
    );
  }


  filter(model: FilterModelWithLanguage): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Content/Filter',
      model,
      { observe: 'response' });
  }

  myContentFilter(model: FilterModelWithLanguage): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Content/MyContentFilter',
      model,
      { observe: 'response' });
  }

  detail(contentId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/Detail?contentId=' + contentId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  myContentDetail(contentId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/MyContentDetail?contentId=' + contentId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/Add',
      { observe: 'response' }
    );
  }

  myContentBeforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/MyContentAdd',
      { observe: 'response' }
    );
  }

  add(model: AddModel<ContentModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Content/Add',
      model,
      { observe: 'response' });
  }

  myContentAdd(model: AddModel<ContentModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Content/MyContentAdd',
      model,
      { observe: 'response' });
  }

  beforeUpdate(contentId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/Update?contentId=' + contentId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  myContentBeforeUpdate(contentId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Content/MyContentUpdate?contentId=' + contentId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }


  update(model: UpdateModel<ContentModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Content/Update',
      model,
      { observe: 'response' });
  }

  myContentUpdate(model: UpdateModel<ContentModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Content/MyContentUpdate',
      model,
      { observe: 'response' });
  }


  delete(contentId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Content/Delete?contentId=' + contentId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  myContentDelete(contentId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Content/MyContentDelete?contentId=' + contentId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

}
