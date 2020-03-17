import { Component, Input, Output, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { ThemeService } from '../../services/theme.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';


@Component({
	selector: 'app-sidebar',
	templateUrl: './sidebar.component.html',
	styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnDestroy , OnInit {

	@Input() sidebarVisible = true;
	@Input() navTab = 'menu';
	@Input() currentActiveMenu: any;
	@Input() currentActiveSubMenu: any;
	@Output() changeNavTabEvent = new EventEmitter();
	@Output() activeInactiveMenuEvent = new EventEmitter();
    public themeClass = 'theme-cyan';
    public darkClass = '';
	private ngUnsubscribe = new Subject();
	private userName = localStorage.getItem('username');
	constructor(private themeService: ThemeService, private router: Router) {
        this.themeService.themeClassChange.pipe(takeUntil(this.ngUnsubscribe)).subscribe(themeClass => {
			this.themeClass = themeClass;
        });
        this.themeService.darkClassChange.pipe(takeUntil(this.ngUnsubscribe)).subscribe(darkClass => {
            this.darkClass = darkClass;
        });
	}
	ngOnInit() {
	  }
    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
	activeInactiveMenu(menuItem: string) {
		this.activeInactiveMenuEvent.emit({ 'item': menuItem });
	}
}
