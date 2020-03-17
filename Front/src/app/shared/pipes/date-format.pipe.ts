import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'DateFormatPipe'
})
export class DateFormatPipe implements PipeTransform {
  transform(value: string) {
    const datePipe = new DatePipe('en-US');
    value = datePipe.transform(value, 'dd-MM-yyyy hh:mm:ss a');
    return value;
  }
}

@Pipe({
  name: 'tradeDatePipe'
})
export class TradeDatePipe implements PipeTransform {
  transform(value, fromDate: Date, toDate: Date, args) {
    if (value === undefined || value == null) {
      return;
    }

    if (args != null) {
      if (args === 0) {
        value = value.filter(function(item) {
          return item;
        });
      } else if (args !== '0') {
        value = value.filter(function(item) {
          return item.Type.toLowerCase().indexOf(args.toLowerCase()) > -1;
        });
      }
    }

    if (fromDate === undefined || toDate === undefined) {
      return value;
    }

    const fd = new Date(fromDate).getDate();
    const td = new Date(toDate).getDate();
    if (fd === td) {
      return value.filter(d => {
        const date = new Date(d.TransactionDate).getDate();
        return fd === date;
      });
    } else {
      return value.filter(d => {
        const date = new Date(d.TransactionDate).getDate();
        return fd <= date && date <= td;
      });
    }
  }
}
