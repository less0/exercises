import { Component, Input } from '@angular/core';
import { GameDetails } from '../model/game-details';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { tap } from 'rxjs';

@Component({
  selector: 'app-display-game',
  templateUrl: './display-game.component.html',
  styleUrls: ['./display-game.component.less']
})
export class DisplayGameComponent {
  @Input() gameDetails! : GameDetails;
  @Input() cardTitle! : string;  

  pinsRolled : number | undefined;

  constructor(private httpClient : HttpClient) {}

  public canRoll() : boolean
  {
    return this.gameDetails!.links!["roll"] != undefined;
  } 

  public roll()
  {
    this.httpClient.post(`${environment.apiUrl}${this.gameDetails!.links!["roll"]}`,{pins: this.pinsRolled})
      .pipe(tap(_ => {
        this.httpClient.get<GameDetails>(`${environment.apiUrl}${this.gameDetails!.links!["self"]}`)
          .pipe(tap(g => this.gameDetails = g))
          .subscribe();
      }))
      .subscribe();
  }
}
