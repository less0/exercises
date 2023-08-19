import { DOCUMENT } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';

@Component({
  selector: 'app-logout-button',
  templateUrl: './logout-button.component.html',
  styleUrls: ['./logout-button.component.less']
})
export class LogoutButtonComponent {
  constructor(
    @Inject(DOCUMENT) public document: Document,
    private auth: AuthService
  ) {}

  logout() {
    this.auth.logout({ 
      logoutParams: {
        returnTo: this.document.location.origin 
      }
    });
  }
}
