import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';



@NgModule({
	imports: [
		CommonModule,
		NgbModule,
		RouterModule,
		FormsModule,
		ReactiveFormsModule
	],
	declarations: [HeaderComponent, SidebarComponent],
	exports: [HeaderComponent, SidebarComponent]
})
export class LayoutModule { }
