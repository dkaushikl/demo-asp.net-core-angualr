import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private language = new Subject<any>();
  language$ = this.language.asObservable();
  constructor() {}
  changeLanguage(val: any) {
    this.language.next(val);
  }
}
