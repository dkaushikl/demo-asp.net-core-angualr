import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import {
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { SpinnerService } from '../../services/spinner.service';

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {
  constructor(private spinnerservice: SpinnerService, public router: Router) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    this.spinnerservice.show();
    return next.handle(request).pipe(
      tap(
        event => {
          if (event instanceof HttpResponse) {
            this.spinnerservice.hide();
            if (event.body.status === 0) {
              localStorage.clear();
              this.router.navigate(['/login']);
            }
          }
        },
        error => {
          this.router.navigate(['/page-404']);
          this.spinnerservice.hide();
        }
      )
    );
  }
}
