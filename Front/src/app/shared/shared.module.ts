import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DataTablesModule } from 'angular-datatables';
import { ToastrModule } from 'ngx-toastr';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { Common } from './common/common';
import { DateFormatPipe, DynamicDigitPipe, TradeDatePipe, DynamicYesNoPipe, FilterDataPipe, SplitStringPipe } from './pipes';
import { DisallowSpaceDirective, NumberOnlyDirective } from './directive';

const pipes = [DateFormatPipe, DynamicYesNoPipe, DynamicDigitPipe, FilterDataPipe,
  SplitStringPipe, TradeDatePipe];

const directives = [DisallowSpaceDirective, NumberOnlyDirective];

@NgModule({
  imports: [
    // Angular
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    // 3rd party
    DataTablesModule,
    ToastrModule.forRoot({ positionClass: 'toast-top-right' , closeButton: true}),
    NgbModule,
    NgMultiSelectDropDownModule
  ],
  declarations: [...pipes, ...directives],
  exports: [
    ...pipes,
    ...directives,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule,
    DataTablesModule,
    NgbModule,
    NgMultiSelectDropDownModule
  ],
  providers: [Common]
})

export class SharedModule { }
