import { Routes, RouterModule } from '@angular/router';
import { AuthenticationComponent } from './authentication/authentication.component';
import { PageLoginComponent } from './page-login/page-login.component';
import { PageForgotPasswordComponent } from './page-forgot-password/page-forgot-password.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { PageRegisterComponent } from './page-register/page-register.component';
import { PageResetPasswordComponent } from './page-reset-password/page-reset-password.component';

const routes: Routes = [
    {
        path: '',
        component: AuthenticationComponent,
        children: [
            { path: '', redirectTo: 'page-login', pathMatch: 'full' },
            { path: 'login', component: PageLoginComponent, data: { title: 'Login' } },
            { path: 'register', component: PageRegisterComponent, data: { title: 'Register' } },
            { path: 'forgot-password', component: PageForgotPasswordComponent,
            data: { title: 'Forgot Password' } },
            { path: 'resetpassword', component: PageResetPasswordComponent, data: { title: 'Reset Password' } },
            { path: 'page-404', component: PageNotFoundComponent, data: { title: 'Page-404' } },
            { path: '**', component: PageNotFoundComponent, data: { title: 'Page-404' } },
        ]
    }
];

export const routing = RouterModule.forChild(routes);
