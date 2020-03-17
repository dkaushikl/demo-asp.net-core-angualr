import { Injectable } from '@angular/core';

@Injectable()
export class OAuthService {
  constructor() {}

  public getAuthorizationHeader(): any {
    if (localStorage.getItem('token')) {
      return 'Bearer ' + localStorage.getItem('token');
    } else {
      return '';
      // this.router.navigate(['/registration/sign-in']);
    }
  }

  public getToken(): any {
    if (localStorage.getItem('token')) {
      return localStorage.getItem('token');
    } else {
      return '';
    }
  }

  public setAuthorizationHeader(value) {
    debugger;
    localStorage.setItem('token', value);
  }
  public setAccountId(value) {
    localStorage.setItem('RelAccountId', value);
  }
  public getAccountId() {
    return localStorage.getItem('RelAccountId');
  }
}
