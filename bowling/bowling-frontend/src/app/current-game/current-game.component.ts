import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, of, tap } from 'rxjs';
import { GameDetails } from '../model/game-details';
import { GamesSummaries } from '../model/games-summaries';

@Component({
  selector: 'app-current-game',
  templateUrl: './current-game.component.html',
  styleUrls: ['./current-game.component.less']
})
export class CurrentGameComponent implements OnInit {
  currentGame$ : Observable<GameDetails | undefined | null> = of(undefined)

  constructor(private httpClient : HttpClient){}

  ngOnInit(): void {
    console.log("Meh");
    this.httpClient.get<GamesSummaries>(`${environment.apiUrl}/games`)
    .pipe(tap(summaries => {
      const linkToCurrent : string | undefined = summaries.links['current'];
      if(linkToCurrent != undefined)
      {
        console.log("Getting current");
        this.currentGame$ = this.httpClient.get<GameDetails>(`${environment.apiUrl}${linkToCurrent}`);
      }
      else
      {
        this.currentGame$ = of(null);
      }
    }))
    .subscribe();
  }
}
