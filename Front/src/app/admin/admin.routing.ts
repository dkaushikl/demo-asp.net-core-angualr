import { Routes, RouterModule } from '@angular/router';
import { AdminComponent } from './admin/admin.component';
import { IndexComponent } from './index/index.component';
import { MUserComponent } from './master/m-user/m-user.component';

const routes: Routes = [
    {
        path: '',
        component: AdminComponent,
        children: [
            { path: '', component: IndexComponent, data: { title: ':: Dashboard' } },
            { path: 'dashboard', component: IndexComponent, data: { title: ':: Dashboard' } },
             {
                path: 'master',
                children : [
                    { path: 'users', component: MUserComponent,
                    data: { title: ':: User' } },
                ]
             }
        ]
    },
];

export const routing = RouterModule.forChild(routes);
