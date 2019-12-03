import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ParameterModel } from 'src/app/models/parameter-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AddModel } from 'src/app/models/add-model';
import { FilterModelWithParent } from 'src/app/models/filter-model-with-parent';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class ParameterService {

  token: string;


  constructor(
    private httpClient: HttpClient,
    private appSettingsService: AppSettingsService,
  ) {

  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Parameter/List', { observe: 'response' }
    );
  }

  filter(model: FilterModelWithParent): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Parameter/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Parameter/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Parameter/Add',
      { observe: 'response' }
    );
  }


  add(model: AddModel<ParameterModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Parameter/Add',
      model,
      { observe: 'response' });
  }



  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Parameter/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<ParameterModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Parameter/Update',
      model,
      { observe: 'response' });
  }


  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Parameter/Delete?id=' + id,
      { observe: 'response' }
    );
  }


}
