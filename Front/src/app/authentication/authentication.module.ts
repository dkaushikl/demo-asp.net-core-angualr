import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageLoginComponent } from './page-login/page-login.component';
import { AuthenticationComponent } from './authentication/authentication.component';
import { routing } from './authentication.routing';
import { PageForgotPasswordComponent } from './page-forgot-password/page-forgot-password.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { PageRegisterComponent } from './page-register/page-register.component';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PageResetPasswordComponent } from './page-reset-password/page-reset-password.component';

@NgModule({
declarations: [PageLoginComponent, AuthenticationComponent, PageRegisterComponent,
PageForgotPasswordComponent, PageNotFoundComponent, PageResetPasswordComponent],
imports: [
CommonModule,
routing,
RouterModule,
FormsModule,
ReactiveFormsModule],
})
export class AuthenticationModule {
 }
