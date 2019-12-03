import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FilterModel } from 'src/app/models/filter-model';
import { AddModel } from 'src/app/models/add-model';
import { UpdateModel } from 'src/app/models/update-model';
import { MenuModel } from 'src/app/models/menu-model';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  constructor(
    private httpClient: HttpClient,
    private appSettingsService: AppSettingsService,
  ) {


  }

  keysAndValues(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Menu/KeysAndValues',
      { observe: 'response' }
    );
  }


  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Menu/List',
      { observe: 'response' }
    );
  }

  filter(model: FilterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Menu/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Menu/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Menu/Add',
      { observe: 'response' }
    );
  }


  add(model: AddModel<MenuModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Menu/Add',
      model,
      { observe: 'response' });
  }



  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Menu/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<MenuModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/Menu/Update',
      model,
      { observe: 'response' });
  }


  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/Menu/Delete?id=' + id,
      { observe: 'response' }
    );
  }
}
