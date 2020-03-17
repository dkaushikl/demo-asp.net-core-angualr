import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'appDynamicDigit'
})
export class DynamicDigitPipe implements PipeTransform {
  transform(value: any, digit: number = 8) {
    if (typeof value === 'string' || value === undefined) {
      return value;
    }
    return value === null ? parseFloat('0').toFixed(digit) : parseFloat(value.toString()).toFixed(digit);
  }
}
