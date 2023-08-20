import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AuthHttpInterceptor, AuthModule } from '@auth0/auth0-angular';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { auth0Configuration } from 'src/environments/auth0';
import { environment } from 'src/environments/environment';

import { LoginButtonComponent } from './login-button/login-button.component';
import { LogoutButtonComponent } from './logout-button/logout-button.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CurrentGameComponent } from './current-game/current-game.component';
import { GamesComponent } from './games/games.component';
import { HomeComponent } from './home/home.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginButtonComponent,
    LogoutButtonComponent,
    CurrentGameComponent,
    GamesComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    AuthModule.forRoot(
      {
        domain: auth0Configuration.domain,
        clientId: auth0Configuration.clientId,

        authorizationParams: {
          redirect_uri: window.location.origin,
          audience: "http://backend:80"
        },

        httpInterceptor: {
          allowedList: [
            {
              // Match any request that starts with our API URL
              uri: `${environment.apiUrl}/*`,
            }
          ],
        },

        useRefreshTokens: true,
        cacheLocation: "localstorage" // TODO This is required to 'survive' refreshes, but another way shall be found, for security reasons
      }
    ),
    NgbModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
