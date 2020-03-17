import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SidebarService } from '../../services/sidebar.service';
import { ToastrService } from 'ngx-toastr';


@Component({
    selector: 'app-index',
    templateUrl: './index.component.html',
    styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {
    public sidebarVisible = true;
    public isResizing = false;
    constructor(private activatedRoute: ActivatedRoute, private sidebarService: SidebarService,
        private cdr: ChangeDetectorRef, private toastr: ToastrService) {
    }
    ngOnInit() {
    }
    toggleFullWidth() {
        this.isResizing = true;
        this.sidebarService.toggle();
        this.sidebarVisible = this.sidebarService.getStatus();
        const that = this;
        setTimeout(function () {
            that.isResizing = false;
            that.cdr.detectChanges();
        }, 400);
    }
}
