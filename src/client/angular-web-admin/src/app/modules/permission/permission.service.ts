import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PermissionModel } from 'src/app/models/permission-model';
import { UpdateModel } from 'src/app/models/update-model';
import { FilterModel } from 'src/app/models/filter-model';
import { AddModel } from 'src/app/models/add-model';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) {

  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Permission/List',
      { observe: 'response' }
    );
  }

  filter(model: FilterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Permission/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Permission/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Permission/Add',
      { observe: 'response' }
    );
  }

  add(model: AddModel<PermissionModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Permission/Add',
      model,
      { observe: 'response' });
  }

  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Permission/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<PermissionModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Permission/Update',
      model,
      { observe: 'response' });
  }

  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Permission/Delete?id=' + id,
      { observe: 'response' }
    );
  }
}
