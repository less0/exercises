import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GamesComponent } from './games/games.component';
import { CurrentGameComponent } from './current-game/current-game.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './auth.guard';

const routes: Routes = [
  {
    path: "games",
    component: GamesComponent,
    canActivate: [authGuard]
  },
  {
    path: "current-game",
    component: CurrentGameComponent,
    canActivate: [authGuard]
  },
  {
    path: "",
    component: HomeComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
