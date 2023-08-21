import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-players-info',
  templateUrl: './players-info.component.html',
  styleUrls: ['./players-info.component.less']
})
export class PlayersInfoComponent {
  @Input() players! : string[]
}
