import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full'},
    { path: '', loadChildren: () => import('../app/admin/admin.module').then(m => m.AdminModule) },
    { path: '', loadChildren: () => import('../app/authentication/authentication.module').then(m => m.AuthenticationModule) },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes, { useHash: false });
