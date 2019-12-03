import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FilterModel } from 'src/app/models/filter-model';
import { AddModel } from 'src/app/models/add-model';
import { LanguageModel } from 'src/app/models/language-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) {

  }

  keysAndValues(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Language/KeysAndValues',
      { observe: 'response' }
    );
  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Language/List',
      { observe: 'response' }
    );
  }

  filter(model: FilterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Language/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Language/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Language/Add',
      { observe: 'response' }
    );
  }

  add(model: AddModel<LanguageModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Language/Add',
      model,
      { observe: 'response' });
  }

  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Language/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<LanguageModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Language/Update',
      model,
      { observe: 'response' });
  }

  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Language/Delete?id=' + id,
      { observe: 'response' }
    );
  }

}
