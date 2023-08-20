import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GamesSummaries } from '../model/games-summaries';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.less']
})
export class GamesComponent implements OnInit {
  gamesSummaries$! : Observable<GamesSummaries>

  public constructor(private httpClient : HttpClient) { }

  ngOnInit(): void {
    this.gamesSummaries$ = this.httpClient.get<GamesSummaries>(`${environment.apiUrl}/games`);
  }
}
