import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { TokenService } from './token.service';

@Injectable()
export class AuthenticationGuard implements CanActivate {
  constructor(private router: Router, private tokenService: TokenService) {}
  canActivate(route: ActivatedRouteSnapshot) {
    const tokendetails = this.tokenService.getTokenDetails();
    const expectedRole: Array<string> = route.data.expectedRole
      ? route.data.expectedRole
      : [];
    if (!this.tokenService.isTokenExpired() && tokendetails) {
      if (expectedRole && expectedRole.length > 0) {
        if (expectedRole.indexOf(tokendetails.role) >= 0 && tokendetails.role) {
          return true;
        } else {
          this.router.navigate(['/registration/unauthorized']);
          return false;
        }
      }
      return true;
    } else {
      this.router.navigate(['/registration/sign-in']);
      return false;
    }
  }
}
