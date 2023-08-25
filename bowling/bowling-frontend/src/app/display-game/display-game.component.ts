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
export class DisplayGameComponent
{
  @Input() gameDetails!: GameDetails;
  @Input() cardTitle!: string;

  public pinsRolled: number | undefined;

  constructor(private httpClient: HttpClient) { }

  public canRoll(): boolean
  {
    return this.gameDetails!.links!["roll"] != undefined;
  }

  public roll()
  {
    if (this.pinsRolled == undefined)
    {
      return;
    }

    this.httpClient.post(`${environment.apiUrl}${this.gameDetails!.links!["roll"]}`, { pins: this.pinsRolled })
      .subscribe(_ =>
      {
        this.pinsRolled = undefined;
        this.httpClient.get<GameDetails>(`${environment.apiUrl}${this.gameDetails!.links!["self"]}`)
          .subscribe(g => this.gameDetails = g);
      });
  }
}
