import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ParameterGroupModel } from 'src/app/models/parameter-group-model';
import { UpdateModel } from 'src/app/models/update-model';
import { FilterModel } from 'src/app/models/filter-model';
import { AddModel } from 'src/app/models/add-model';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class ParameterGroupService {


  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
  ) {

  }

  keysAndValues(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/ParameterGroup/KeysAndValues',
      { observe: 'response' }
    );
  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/ParameterGroup/List',
      { observe: 'response' }
    );
  }

  filter(model: FilterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/ParameterGroup/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/ParameterGroup/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/ParameterGroup/Add',
      { observe: 'response' }
    );
  }


  add(model: AddModel<ParameterGroupModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/ParameterGroup/Add',
      model,
      { observe: 'response' });
  }



  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/ParameterGroup/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<ParameterGroupModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/ParameterGroup/Update',
      model,
      {  observe: 'response' });
  }


  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/ParameterGroup/Delete?id=' + id,
      { observe: 'response' }
    );
  }


}
