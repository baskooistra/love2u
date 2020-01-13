import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AuthorizationService, AuthenticationResultStatus } from '../authorization.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthorizationPaths, LogoutActions } from '../authorization.constants';
import { take } from 'rxjs/operators';

@Component({
  template: `
    <p>{{ errorMessage | async }}</p>`,
  styles: [],
  encapsulation: ViewEncapsulation.None
})
export class LogoutComponent implements OnInit {

  public errorMessage = new BehaviorSubject<string>(null);

  constructor(private authorizeService: AuthorizationService, private activatedRoute: ActivatedRoute, private router: Router) {
  }

  async ngOnInit() {
    const action = this.activatedRoute.snapshot.url[1];
    switch (action.path) {
      case LogoutActions.Logout:
        if (!!window.history.state.local) {
          await this.logout(this.getReturnUrl());
        } else {
          // This prevents regular links to <app>/authentication/logout from triggering a logout
          this.errorMessage.next('The logout was not initiated from within the page.');
        }

        break;
      case LogoutActions.LogoutCallback:
        await this.processLogoutCallback();
        break;
      case LogoutActions.LoggedOut:
        this.errorMessage.next('You successfully logged out!');
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }

  private async logout(returnUrl: string): Promise<void> {
    const state: NavigationState = { returnUrl };
    const isauthenticated = await this.authorizeService.isAuthenticated().pipe(
      take(1)
    ).toPromise();
    if (isauthenticated) {
      const result = await this.authorizeService.signOut(state);
      switch (result.status) {
        case AuthenticationResultStatus.Redirect:
          break;
        case AuthenticationResultStatus.Success:
          await this.navigateToReturnUrl(returnUrl);
          break;
        case AuthenticationResultStatus.Fail:
          this.errorMessage.next(result.message);
          break;
        default:
          throw new Error('Invalid authentication result status.');
      }
    } else {
      this.errorMessage.next('You successfully logged out!');
    }
  }

  private async processLogoutCallback(): Promise<void> {
    const url = window.location.href;
    const result = await this.authorizeService.completeSignOut(url);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as the only time completeAuthentication finishes
        // is when we are doing a redirect sign in flow.
        throw new Error('Should not redirect.');
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(this.getReturnUrl(result.state));
        break;
      case AuthenticationResultStatus.Fail:
        this.errorMessage.next(result.message);
        break;
      default:
        throw new Error('Invalid authentication result status.');
    }
  }

  private async navigateToReturnUrl(returnUrl: string) {
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }

  private getReturnUrl(state?: NavigationState): string {
    const fromQuery = (this.activatedRoute.snapshot.queryParams as NavigationState).returnUrl;
    // If the url is comming from the query string, check that is either
    // a relative url or an absolute url
    if (fromQuery &&
      !(fromQuery.startsWith(`${window.location.origin}/`) ||
        /\/[^\/].*/.test(fromQuery))) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }
    return (state && state.returnUrl) ||
      fromQuery ||
      AuthorizationPaths.LOGGED_OUT;
  }
}

interface NavigationState {
  returnUrl: string;
}
