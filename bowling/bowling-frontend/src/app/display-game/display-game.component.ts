import { Component, Input } from '@angular/core';
import { GameDetails } from '../model/game-details';

@Component({
  selector: 'app-display-game',
  templateUrl: './display-game.component.html',
  styleUrls: ['./display-game.component.less']
})
export class DisplayGameComponent {
  @Input() gameDetails! : GameDetails;
  @Input() cardTitle! : string;  
}
