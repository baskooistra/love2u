import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AuthorizationService, AuthenticationResult, AuthenticationResultStatus } from '../authorization.service';
import { AuthorizationPaths, LoginActions } from '../authorization.constants';

@Component({
  template: `
    <p>{{ errorMessage | async }}</p>
  `,
  styles: [],
  encapsulation: ViewEncapsulation.None
})
export class LoginComponent implements OnInit {
  public errorMessage = new BehaviorSubject<string>(null);

  constructor(private authorizeService: AuthorizationService, private activatedRoute: ActivatedRoute, private router: Router) {

  }

  async ngOnInit() {
    const action = this.activatedRoute.snapshot.url[1];
    switch (action.path) {
      case LoginActions.Login:
        await this.login(this.getReturnUrl());
        break;
      case LoginActions.LoginCallback:
        await this.processLoginCallback();
        break;
      case LoginActions.LoginFailed:
        const message = this.activatedRoute.snapshot.queryParams.message;
        this.errorMessage.next(message);
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }

  private getReturnUrl(state?: NavigationState): string {
    const fromQuery = this.activatedRoute.snapshot.queryParams.returnUrl;
    // If the url is comming from the query string, check that is either 
    // a relative url or an absolute url
    if (fromQuery &&
      !(fromQuery.startsWith(`${window.location.origin}/`) ||
        /\/[^\/].*/.test(fromQuery))) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }
    return (state && state.returnUrl) || fromQuery || `window.location.origin${'/'}`;
  }

  private async login(returnUrl: string): Promise<void> {
    const state: NavigationState = { returnUrl };
    const result: AuthenticationResult = await this.authorizeService.signIn(state);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        break;
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(returnUrl);
        break;
      case AuthenticationResultStatus.Fail:
        await this.router.navigate([AuthorizationPaths.LOGIN_FAILED], {
          queryParams: {
            message: result.message
          }
        });
        break;
      default:
        throw new Error(`Invalid status result ${(result as any).status}.`);
    }
  }

  private async processLoginCallback(): Promise<void> {
    const url = window.location.href;
    const result = await this.authorizeService.completeSignIn(url);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as completeSignIn never redirects.
        throw new Error('Should not redirect.');
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(this.getReturnUrl(result.state));
        break;
      case AuthenticationResultStatus.Fail:
        this.errorMessage.next(result.message);
        break;
    }
  }

  private async navigateToReturnUrl(returnUrl: string) {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }
}

interface NavigationState {
  returnUrl: string;
}
