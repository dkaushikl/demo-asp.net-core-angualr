import { Injectable } from '@angular/core';
import { UserInfoModel } from 'models/custom.model';
import * as moment from 'moment';
import { OAuthService } from './o-auth.service';

@Injectable()
export class TokenService {
  jwtDecode = require('jwt-decode');
  constructor(private oAuthService: OAuthService) {}
  getUserInfo(): UserInfoModel {
    const token = this.oAuthService.getToken();

    if (token && token.length > 0) {
      const userInfoModel: UserInfoModel = new UserInfoModel();
      userInfoModel.firstName = localStorage.getItem('firstName');
      userInfoModel.lastName = localStorage.getItem('lastName');
      userInfoModel.email = localStorage.getItem('email');
      userInfoModel.phoneNumber = localStorage.getItem('phoneNumber');
      userInfoModel.onLeave = JSON.parse(localStorage.getItem('onLeave'));
      userInfoModel.leaveReason = localStorage.getItem('reason');
      const decoded = this.jwtDecode(token);
      if (decoded && decoded.role) {
        userInfoModel.role =
          decoded.role === 'Institute Adm'
            ? 'Institution Admin'
            : decoded.role === 'superadmin'
            ? 'Super Admin'
            : decoded.role;
      }
      return userInfoModel;
    } else {
      return null;
    }
  }
  getTokenDetails(): any {
    const token = this.oAuthService.getToken();
    const decoded =
      token && token.length > 0 && token !== 'null'
        ? this.jwtDecode(token)
        : null;
    if (decoded) {
      return decoded;
    }
    return null;
  }
  isTokenExpired(): boolean {
    const token = this.oAuthService.getToken();
    const decoded =
      token && token.length > 0 && token !== 'null'
        ? this.jwtDecode(token)
        : null;
    if (decoded) {
      const day = moment.unix(decoded.exp);
      if (day.toDate() <= moment.default(Date.now()).toDate()) {
        return true;
      } else {
        return false;
      }
    }
    return true;
  }
}
