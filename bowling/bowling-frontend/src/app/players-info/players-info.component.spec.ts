import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayersInfoComponent } from './players-info.component';

describe('PlayersInfoComponent', () => {
  let component: PlayersInfoComponent;
  let fixture: ComponentFixture<PlayersInfoComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PlayersInfoComponent]
    });
    fixture = TestBed.createComponent(PlayersInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
