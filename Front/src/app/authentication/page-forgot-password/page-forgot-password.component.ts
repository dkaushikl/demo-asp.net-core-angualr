import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthClient } from 'api/apiclient';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-page-forgot-password',
  templateUrl: './page-forgot-password.component.html',
  styleUrls: ['./page-forgot-password.component.css']
})
export class PageForgotPasswordComponent {
  public forgotForm: FormGroup;
  isValid: false;
  isSubmitted = false;

  constructor(
    private router: Router,
    private client: AuthClient,
    private toastr: ToastrService
  ) {
    this.bindForm();
  }

  bindForm() {
    this.forgotForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email])
    });
  }
  onSubmit(obj: any, isValid: boolean) {
    this.isSubmitted = true;
    if (isValid) {
      this.client.forgotPassword(obj.email).subscribe(result => {
        if (result.successful) {
          this.toastr.success('We have sent reset password link to your email account.', 'Success');
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1000);
        } else {
          let error = '';
          result.errorMessages.map(
            (item, i) =>
              (error +=
                i !== 0 ? '<br/>' + item.errorMessage : item.errorMessage)
          );
          this.toastr.error(error, 'Alert');
        }
      });
    }
  }
}
