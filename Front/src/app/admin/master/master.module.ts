import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule  } from './../../shared/index';
import { MUserComponent } from './m-user/m-user.component';



@NgModule({
	imports: [
		RouterModule,
		SharedModule
	],
	declarations: [
	MUserComponent,
	],
	exports: []
})
export class Master { }
