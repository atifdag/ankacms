import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UpdatePasswordModel } from 'src/app/models/update-password-model';
import { UpdateMyInformationModel } from 'src/app/models/update-my-information-model';
import { UserModel } from 'src/app/models/user-model';
import { UpdateModel } from 'src/app/models/update-model';
import { AddModel } from 'src/app/models/add-model';
import { AppSettingsService } from 'src/app/app-settings.service';
import { FilterModelWithMultiParent } from 'src/app/models/filter-model-with-multi-parent';

@Injectable({
  providedIn: 'root'
})
export class UserService {


  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient) {
  }

  myProfile(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/User/MyProfile',
      { observe: 'response' }
    );
  }

  updateMyPassword(model: UpdatePasswordModel): Observable<HttpResponse<any>> {
    return this.httpClient.put(this.appSettingsService.apiUrl + '/User/UpdateMyPassword', model, { observe: 'response' });
  }

  updateMyInformation(model: UpdateMyInformationModel): Observable<HttpResponse<any>> {
    return this.httpClient.put(this.appSettingsService.apiUrl + '/User/UpdateMyInformation', model, { observe: 'response' });
  }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/User/List',
      { observe: 'response' }
    );
  }

  filter(model: FilterModelWithMultiParent): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/User/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/User/Detail?id=' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/User/Add',
      { observe: 'response' }
    );
  }


  add(model: AddModel<UserModel>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/User/Add',
      model,
      { observe: 'response' });
  }



  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/User/Update?id=' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<UserModel>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/User/Update',
      model,
      { observe: 'response' });
  }


  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/User/Delete?id=' + id,
      { observe: 'response' }
    );
  }


}
