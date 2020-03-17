import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgbDropdownConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ThemeService } from '../../services/theme.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { AuthClient } from 'api/apiclient';
import { OAuthService } from '../../services/o-auth.service';
import { ToastrService } from 'ngx-toastr';


@Component({
	selector: 'app-header',
	templateUrl: './header.component.html',
	styleUrls: ['./header.component.css'],
	providers: [NgbDropdownConfig]
})
export class HeaderComponent implements OnInit {

	// Properties
	public changeForm: FormGroup;
	isValid: false;
	isSubmitted = false;
	@Input() showNotifMenu = false;
	@Input() showToggleMenu = false;
	@Input() darkClass = '';
	@Output() toggleSettingDropMenuEvent = new EventEmitter();
	@Output() toggleNotificationDropMenuEvent = new EventEmitter();
	sellang: string;
	constructor(private config: NgbDropdownConfig, private themeService: ThemeService,
		private modalService: NgbModal, private router: Router, private oAuthService: OAuthService,
    private toastr: ToastrService,
	private client: AuthClient) {
		config.placement = 'bottom-right';
		this.bindForm();
	}
	bindForm() {
		this.changeForm = new FormGroup({
		  oldpassword: new FormControl('', [Validators.required]),
		  password: new FormControl('', [Validators.required]),
		  confirmpassword: new FormControl('', Validators.compose([
            Validators.required,
            this.matchConfirmPassword,
          ])),
		});
	  }
	  matchConfirmPassword(control: FormControl) {
        if (control.parent !== undefined) {
          if (control.parent.value.password !== '' && control.value !== '') {
            const password = control.parent.value.password;
            const confirmPassword = control.value;
            if (password === confirmPassword) {
              return null;
            } else {
              return { matchConfirmPassword: true };
            }
          }
        }
      }
	ngOnInit() {
	}
	toggleSettingDropMenu() {
		this.toggleSettingDropMenuEvent.emit();
	}

	toggleNotificationDropMenu() {
		this.toggleNotificationDropMenuEvent.emit();
	}

	toggleSideMenu() {
		this.themeService.showHideMenu();
	}

	openModal(content: any, size: any) {
		this.modalService.open(content, { size: size });
	}
	onSubmit(obj: any, isValid: boolean) {
		this.isSubmitted = true;
		if (isValid) {
		  this.client.changePassword(obj.oldpassword, obj.password).subscribe(result => {
			if (result.successful) {
			  this.toastr.success('Password change successfully', 'Success');
			  this.modalService.dismissAll();
			  localStorage.clear();
			 this.router.navigate(['/']);
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
	  }
	logOut() {
		Swal.fire({
			title: 'Are you sure?',
			text: 'Are you sure you want to log out?',
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Yes',
			cancelButtonText: 'No, keep it'
		  }).then((result) => {
			if (result.value) {
			localStorage.clear();
			this.router.navigate(['/']);
			}
		  });
	}
}
