import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FramesTableComponent } from './frames-table.component';

describe('FramesTableComponent', () => {
  let component: FramesTableComponent;
  let fixture: ComponentFixture<FramesTableComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [FramesTableComponent]
    });
    fixture = TestBed.createComponent(FramesTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
