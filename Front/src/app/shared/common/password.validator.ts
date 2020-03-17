import { FormControl } from '@angular/forms';

export interface IValidationResult {
    [key: string]: boolean;
}

export class PasswordValidator {
    public static strong(control: FormControl): IValidationResult {
        const hasNumber = /\d/.test(control.value);
        const hasUpper = /[A-Z]/.test(control.value);
        const hasLower = /[a-z]/.test(control.value);
        const hasSymbol = /[!@#$&*]/.test(control.value);
        const valid = hasNumber && hasUpper && hasLower && hasSymbol;
        if (!valid) {
            return { strong: true };
        }
        return null;
    }

    public static checkPassword(control: FormControl): IValidationResult {
        if (control.parent !== undefined) {
            if (control.parent.value.Password !== '' && control.value !== '') {
                const password = control.parent.value.Password;
                const confirmPassword = control.value;
                if (password === confirmPassword) {
                    return null;
                } else {
                    return { matchPassword: true };
                }
            }
        }
    }

    public static MatchPassword(control: FormControl): IValidationResult {
        if (control.parent !== undefined) {
            if (control.parent.value.NewPassword !== '' && control.value !== '') {
                if (control.parent.value.NewPassword === control.value) {
                    return null;
                } else {
                    return { matchPassword: true };
                }
            }
        }
    }
}

// https://scotch.io/@ibrahimalsurkhi/match-password-validation-with-angular-2

// https://stackoverflow.com/questions/48350506/how-to-validate-password-strength-with-angular-5-validator-pattern


