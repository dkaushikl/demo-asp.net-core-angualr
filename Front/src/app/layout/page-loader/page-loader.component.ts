import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-page-loader',
  templateUrl: './page-loader.component.html',
  styleUrls: ['./page-loader.component.css']
})

export class PageLoaderComponent {
  isLoading: Subject<boolean>;

  constructor(private spinnerservice: SpinnerService) {
    this.isLoading = this.spinnerservice.isLoading;
  }
}
