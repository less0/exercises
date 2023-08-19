import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentGameComponent } from './current-game.component';

describe('CurrentGameComponent', () => {
  let component: CurrentGameComponent;
  let fixture: ComponentFixture<CurrentGameComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CurrentGameComponent]
    });
    fixture = TestBed.createComponent(CurrentGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
