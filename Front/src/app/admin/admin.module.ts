import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LayoutModule } from '../layout/layout.module';
import { routing } from './admin.routing';
import { AdminComponent } from './admin/admin.component';
import { IndexComponent } from './index/index.component';
import { Master } from './master/master.module';
import { SharedModule  } from './../shared/index';


@NgModule({
	imports: [
		routing,
		LayoutModule,
		RouterModule,
		SharedModule,
		Master
	],
	declarations: [
		AdminComponent,
		IndexComponent,
	]
})
export class AdminModule { }
