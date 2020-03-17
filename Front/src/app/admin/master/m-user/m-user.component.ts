import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { SidebarService } from '../../../services/sidebar.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { AuthClient } from 'api/apiclient';
import { OAuthService } from '../../../services/o-auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-m-user',
  templateUrl: './m-user.component.html',
  styleUrls: ['./m-user.component.css']
})
export class MUserComponent implements OnInit {
  public sidebarVisible = true;
  dtOptions: DataTables.Settings = {};
  public searchForm: FormGroup;
  public createForm: FormGroup;
  isSubmitted = false;
  userList = [];
  constructor(
    private sidebarService: SidebarService,
    private cdr: ChangeDetectorRef,
    private modalService: NgbModal,
    private toastr: ToastrService,
    private oAuthService: OAuthService,
    private client: AuthClient
  ) {}
  ngOnInit(): void {
    this.dtOptions = {
      paging: false,
      lengthChange: false,
      ordering: false,
      searching: false,
      info: false,
      columns: [
        { width: '20%' },
        { width: '40%' },
        { width: '40%' }
      ]
    };
    this.getData();
  }
  getData() {
    this.client.getAllUsers().subscribe(result => {
      if (result.successful) {
        this.userList = result.data;
      } else {
        let error = '';
        result.errorMessages.map(
          (item, i) =>
            (error += i !== 0 ? '<br/>' + item.errorMessage : item.errorMessage)
        );
        this.toastr.error(error, 'Alert');
      }
    });
  }
  toggleFullWidth() {
    this.sidebarService.toggle();
    this.sidebarVisible = this.sidebarService.getStatus();
    this.cdr.detectChanges();
  }
  deleteUser(id) {
    Swal.fire({
			title: 'Are you sure?',
			text: 'Are you sure you want to delete?',
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Yes',
			cancelButtonText: 'No, keep it'
		  }).then((result) => {
			if (result.value) {
			    this.client.deleteUser(id).subscribe(res => {
          if (res.successful) {
            Swal.fire(
              'Deleted!',
              'success'
              );
            this.getData();
          } else {
            let error = '';
            res.errorMessages.map(
              (item, i) =>
                (error += i !== 0 ? '<br/>' + item.errorMessage : item.errorMessage)
            );
            this.toastr.error(error, 'Alert');
          }
        });
			}
		  });
  }
  activedeactive(id, status) {
    const currStatus = status === true ? 'Enable' : 'Disable';
    Swal.fire({
			title: 'Are you sure?',
			text: 'Are you sure you want to ' + currStatus + ' user?',
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Yes',
			cancelButtonText: 'No, keep it'
		  }).then((result) => {
			if (result.value) {

        if (status === true) {

          this.client.activeUser(id).subscribe(res => {
            if (res.successful) {
              Swal.fire(
                'Status!',
                'change status success',
                'success'
                );
                this.getData();
            } else {
              let error = '';
              res.errorMessages.map(
                (item, i) =>
                  (error += i !== 0 ? '<br/>' + item.errorMessage : item.errorMessage)
              );
              this.toastr.error(error, 'Alert');
            }
          });
        } else {
          this.client.disableUser(id).subscribe(res => {
            if (res.successful) {
              Swal.fire(
                'Status!',
                'change status success',
                'success'
                );
                this.getData();
            } else {
              let error = '';
              res.errorMessages.map(
                (item, i) =>
                  (error += i !== 0 ? '<br/>' + item.errorMessage : item.errorMessage)
              );
              this.toastr.error(error, 'Alert');
            }
          });
        }
			}
		  });
  }
}
