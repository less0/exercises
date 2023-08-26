import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GamesSummaries } from '../model/games-summaries';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameDetails } from '../model/game-details';
import { GameSummary } from '../model/game-summary';

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.less']
})
export class GamesComponent implements OnInit
{
  gamesSummaries$!: Observable<GamesSummaries>
  selectedGame: GameDetails | undefined

  public constructor(private httpClient: HttpClient) { }

  ngOnInit(): void
  {
    this.gamesSummaries$ = this.httpClient.get<GamesSummaries>(`${environment.apiUrl}/games`);
  }

  public selectGame(summary : GameSummary) : void
  {
    const detailsLink = summary.links!["details"];
    if(detailsLink == undefined)
    {
      console.log("Details link is undefined");
      
      return;
    }

    console.log(`Getting ${detailsLink}`);
    
    this.httpClient.get<GameDetails>(`${environment.apiUrl}${detailsLink}`)
                   .subscribe(gameDetails => this.selectedGame = gameDetails);
  }

  public unselectGame() : void
  {
    this.selectedGame = undefined;
  }
}
