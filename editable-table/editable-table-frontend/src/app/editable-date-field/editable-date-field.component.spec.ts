import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditableDateFieldComponent } from './editable-date-field.component';

describe('EditableDateFieldComponent', () => {
  let component: EditableDateFieldComponent;
  let fixture: ComponentFixture<EditableDateFieldComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EditableDateFieldComponent]
    });
    fixture = TestBed.createComponent(EditableDateFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
