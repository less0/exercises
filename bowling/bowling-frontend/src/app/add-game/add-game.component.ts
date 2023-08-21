import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { GamesSummaries } from '../model/games-summaries';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-game',
  templateUrl: './add-game.component.html',
  styleUrls: ['./add-game.component.less']
})
export class AddGameComponent
{
  validateForm = false;
  form = this.formBuilder.group({ players: this.formBuilder.array([]) });

  constructor(private formBuilder: FormBuilder, private httpClient: HttpClient, private router: Router) { }

  get players(): FormArray
  {
    return this.form.controls["players"] as FormArray;
  }

  get playerNames(): string[]
  {
    const players: string[] = this.players.controls.map<string>(control =>
    {
      var formGroup = control as FormGroup;
      var nameControl = formGroup.controls["name"] as FormControl;

      if (nameControl.invalid)
      {
        throw "Player name control is invalid";
      }

      return nameControl.value;
    });
    return players;
  }

  public addPlayer()
  {
    const player = this.formBuilder.group(
      { name: ['', Validators.required] }
    )

    this.players.push(player);
  }

  public startGame()
  {
    this.validateForm = true;
    if (this.form.invalid)
    {
      return;
    }

    this.httpClient.get<GamesSummaries>(`${environment.apiUrl}/games`)
      .subscribe(gamesSummaries =>
      {
        if (gamesSummaries.links["start"])
        {
          this.httpClient.post(`${environment.apiUrl}${gamesSummaries.links["start"]}`, this.playerNames)
            .subscribe(() =>
            {
              this.router.navigateByUrl('/', { skipLocationChange: true })
                .then(() => this.router.navigateByUrl("current-game"));
            });
        }
        else
        {
          console.error("start link not available");
        }
      });
  }
}
