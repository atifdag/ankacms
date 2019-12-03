import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FilterModel } from 'src/app/models/filter-model';
import { AddModel } from 'src/app/models/add-model';
import { RoleModel } from 'src/app/models/role-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) {

  }

  keysAndValues(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Role/KeysAndValues',
      { observe: 'response' }
    );
  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Role/List',
      { observe: 'response' }
    );
  } 

  filter(model: FilterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Role/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Role/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Role/Add',
      { observe: 'response' }
    );
  }

  add(model: AddModel<RoleModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Role/Add',
      model,
      { observe: 'response' });
  }

  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Role/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<RoleModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Role/Update',
      model,
      { observe: 'response' });
  }

  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Role/Delete?id=' + id,
      { observe: 'response' }
    );
  }

}
