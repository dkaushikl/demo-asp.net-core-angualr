import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MUserComponent } from './m-user.component';

describe('MUserComponent', () => {
  let component: MUserComponent;
  let fixture: ComponentFixture<MUserComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MUserComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
