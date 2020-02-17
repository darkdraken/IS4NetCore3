import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { JwksValidationHandler, OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { authConfig } from './auth.config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  location = window.location.origin;
  apiResult$: Observable<any>;
  get hasValidToken(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  get accessToken(): string {
    return this.oauthService.getAccessToken();
  }

  constructor(private oauthService: OAuthService,
              private http: HttpClient,
  ) {
    this.configure();
    this.callApi();
  }

  configure() {
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.loadDiscoveryDocumentAndTryLogin({
      disableOAuth2StateCheck: true
    });
    if (!this.hasValidToken) {
      this.oauthService.initImplicitFlow();
    }
  }

  callApi() {
    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.apiResult$ = this.http.get<any>('http://localhost:5001/identity', { headers });
  }

}
