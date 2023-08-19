import { Component } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';

@Component({
  selector: 'app-login-button',
  templateUrl: './login-button.component.html',
  styleUrls: ['./login-button.component.less']
})
export class LoginButtonComponent {
public constructor(private auth : AuthService){}

login() {
this.auth.loginWithRedirect();
}

}
