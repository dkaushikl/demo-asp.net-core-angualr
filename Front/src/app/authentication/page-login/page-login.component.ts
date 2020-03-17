import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthClient, LoginModel } from 'api/apiclient';
import { OAuthService } from '../../services/o-auth.service';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-page-login',
  templateUrl: './page-login.component.html',
  styleUrls: ['./page-login.component.css']
})
export class PageLoginComponent implements OnInit {
  public loginForm: FormGroup;
  isValid: false;
  isSubmitted = false;
  model = new LoginModel();
  constructor(private router: Router, private client: AuthClient, private toastr: ToastrService, private oAuthService: OAuthService) {
    this.bindForm();
  }
  ngOnInit() {
  }
  bindForm() {
    this.loginForm = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required])
    });
  }
  onSubmit(obj: any, isValid: boolean) {
    this.isSubmitted = true;
    if (isValid) {
      this.model.email = obj.username;
      this.model.password = obj.password;
      this.client.login(this.model).subscribe(result => {
        if (result.successful) {
          if (result.data.token) {
            this.oAuthService.setAuthorizationHeader(result.data.token);
          }
          this.toastr.success('Login successfully', 'Success');
          localStorage.setItem(
            'username',
            result.data.firstName ? result.data.firstName : ''
          );
          localStorage.setItem(
            'userid',
            String(result.data.id)
          );
         this.router.navigate(['/dashboard']);
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
