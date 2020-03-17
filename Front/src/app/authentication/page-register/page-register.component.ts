import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute  } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthClient, RegisterModel } from 'api/apiclient';
import { OAuthService } from '../../services/o-auth.service';



@Component({
    selector: 'app-page-register',
    templateUrl: './page-register.component.html',
    styleUrls: ['./page-register.component.css']
})
export class PageRegisterComponent implements OnInit {
    public registerForm: FormGroup;
    isSubmitted = false;
    model = new RegisterModel();
    isValid: false;
    userID: string;
    code: string;

    constructor(private router: Router, private toastr: ToastrService, private client: AuthClient,
      private oAuthService: OAuthService, private route: ActivatedRoute) {
        this.bindForm();
     }

    bindForm() {
        this.registerForm = new FormGroup({
          username: new FormControl('', [Validators.required]),
          firstname: new FormControl('', [Validators.required]),
          lastname: new FormControl('', [Validators.required]),
          email: new FormControl('', [Validators.required, Validators.email]),
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
      this.userID = this.route.snapshot.queryParamMap.get('userid');
      this.code = this.route.snapshot.queryParamMap.get('code');
      if (this.userID !== null && this.code !== null) {
        this.client.confirmEmail(this.userID, this.code).subscribe(result => {
          if (result.successful) {
            this.toastr.success('Email verify successfully', 'Success');
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

    onSubmit(obj: any, isValid: boolean) {

        this.isSubmitted = true;
        if (isValid) {
          this.model.userName = obj.username;
          this.model.firstName = obj.firstname;
          this.model.lastName = obj.lastname;
          this.model.email = obj.email;
          this.model.password = obj.password;
          this.model.confirmPassword = obj.confirmpassword;
          this.client.register(this.model).subscribe(result => {
            if (result.successful) {
              this.toastr.success('Register successfully and verification link has been sent to your email account.', 'Success');
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
