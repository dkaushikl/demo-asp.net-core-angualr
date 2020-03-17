// import { TokenService } from '../services/token.service';
// import { AuthenticationGuard } from './services/authentication.guard';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthClient } from 'api/apiclient';
import { Globals } from 'app/globals';
import { SpinnerService } from 'app/services/spinner.service';
import { AppComponent } from './app.component';
import { routing } from './app.routing';
import { HttpConfigInterceptor } from './core/interceptors/interceptors';
import { PageLoaderComponent } from './layout/page-loader/page-loader.component';
import { OAuthService } from './services/o-auth.service';
import { SharedModule } from './shared/index';

@NgModule({
    declarations: [
        AppComponent,
        PageLoaderComponent
    ],
    imports: [
        HttpClientModule,
        BrowserModule,
        routing,
        BrowserAnimationsModule,
        SharedModule
    ],
    providers: [AuthClient, OAuthService, SpinnerService,
        {
          provide: HTTP_INTERCEPTORS,
          useClass: HttpConfigInterceptor,
          multi: true
        }],
    bootstrap: [AppComponent]
})
export class AppModule {
    constructor(private injector: Injector) {
        Globals.injector = this.injector;
      }
}
