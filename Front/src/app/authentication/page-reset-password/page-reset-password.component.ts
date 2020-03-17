import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthClient } from 'api/apiclient';
import { OAuthService } from '../../services/o-auth.service';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-page-reset-password',
  templateUrl: './page-reset-password.component.html',
  styleUrls: ['./page-reset-password.component.css']
})
export class PageResetPasswordComponent implements OnInit {
  public resetForm: FormGroup;
  isValid: false;
  isSubmitted = false;
  userID: string;
  code: string;
  constructor(private router: Router, private client: AuthClient, private toastr: ToastrService, private oAuthService: OAuthService,
    private route: ActivatedRoute) {
    this.bindForm();
  }
  ngOnInit() {
    this.userID = this.route.snapshot.queryParamMap.get('userid');
    this.code = this.route.snapshot.queryParamMap.get('code');
  }
  bindForm() {
    this.resetForm = new FormGroup({
      password: new FormControl('', [Validators.required]),
      confirmpassword: new FormControl('', [Validators.required])
    });
  }
  onSubmit(obj: any, isValid: boolean) {
    this.isSubmitted = true;
    if (isValid) {
      this.client.resetPassword(this.code, obj.confirmpassword, obj.password, this.userID).subscribe(result => {
        if (result.successful) {
        this.toastr.success('Your password reset successfully', 'Success');
         this.router.navigate(['/login']);
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
}
