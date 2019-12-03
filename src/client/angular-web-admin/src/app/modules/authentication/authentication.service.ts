import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { SignOutOption } from 'src/app/value-objects/sign-out-option.enum';
import { Observable } from 'rxjs';
import { SignUpModel } from 'src/app/models/sign-up-model';
import { SignInModel } from 'src/app/models/sign-in-model';
import { IdentityService } from 'src/app/identity.service';
import { AppSettingsService } from 'src/app/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(
    private svcAppSet: AppSettingsService,
    private httpClient: HttpClient,
    private svcIdentity: IdentityService,
  ) { }

  signIn(model: SignInModel): Observable<HttpResponse<any>> {
    model.key = this.svcAppSet.jwtSecurityKey;
    return this.httpClient.post(this.svcAppSet.apiUrl + '/Authentication/SignIn', model, { observe: 'response' });
  }

  signOut(signOutOption: SignOutOption) {
    this.svcIdentity.remove();
    return this.httpClient.get(this.svcAppSet.apiUrl + '/Authentication/SignOut?signOutOption=' + signOutOption);
  }

  forgotPassword(username: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(this.svcAppSet.apiUrl + '/Authentication/ForgotPassword' + '?username=' + username, { observe: 'response' });
  }


  signUp(model: SignUpModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(this.svcAppSet.apiUrl + '/Authentication/SignUp', model, { observe: 'response' });
  }
}
