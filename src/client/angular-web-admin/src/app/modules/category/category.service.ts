import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoryModel } from 'src/app/models/category-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AddModel } from 'src/app/models/add-model';
import { AppSettingsService } from 'src/app/app-settings.service';
import { FilterModelWithLanguage } from 'src/app/models/filter-model-with-language';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(
    private httpClient: HttpClient,
    private appSettingsService: AppSettingsService,
  ) {
  }

  keysAndValues(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Category/KeysAndValues',
      { observe: 'response' }
    );
  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Category/List', { observe: 'response' }
    );
  }

  filter(model: FilterModelWithLanguage): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Category/Filter',
      model,
      { observe: 'response' });
  }

  detail(categoryId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Category/Detail?categoryId=' + categoryId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Category/Add',
      { observe: 'response' }
    );
  }

  add(model: AddModel<CategoryModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Category/Add',
      model,
      { observe: 'response' });
  }

  beforeUpdate(categoryId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Category/Update?categoryId=' + categoryId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<CategoryModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Category/Update',
      model,
      { observe: 'response' });
  }

  delete(categoryId: string, languageId: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Category/Delete?categoryId=' + categoryId + '&languageId=' + languageId,
      { observe: 'response' }
    );
  }


}
