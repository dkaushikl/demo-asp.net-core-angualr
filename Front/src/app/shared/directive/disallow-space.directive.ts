import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appDisallowSpaces]'
})

export class DisallowSpaceDirective {
  private regex: RegExp = new RegExp(/\s/g);

  private specialKeys: Array<string> = ['Space'];

  constructor(private el: ElementRef) { }

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
      const current: string = this.el.nativeElement.value;
      if (event.keyCode === 32) {
          return false;
      }
      const next: string = current.concat(event.key);
  }
}
