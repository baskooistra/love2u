import { Injectable } from '@angular/core';
import { Log, UserManager, User, UserManagerSettings } from 'oidc-client';
import { BehaviorSubject, Observable, from, concat } from 'rxjs';
import { filter, map, mergeMap, take, tap } from 'rxjs/operators';
import { AuthorizationPaths, AuthorizationConfiguration } from './authorization.constants';
import { debug } from 'util';

export class Love2uUser {
  id: string;
  firstName: string;
  lastName: string;
}

export type AuthenticationResult =
  SuccessAuthenticationResult |
  FailureAuthenticationResult |
  RedirectAuthenticationResult;

export class SuccessAuthenticationResult {
  status: AuthenticationResultStatus.Success;
  state: any;
}

export class FailureAuthenticationResult {
  status: AuthenticationResultStatus.Fail;
  message: string;
}

export class RedirectAuthenticationResult {
  status: AuthenticationResultStatus.Redirect;
}

export enum AuthenticationResultStatus {
  Success,
  Redirect,
  Fail
}

@Injectable({
  providedIn: 'root'
})

export class AuthorizationService {
  private popUpDisabled = true;
  private userManager: UserManager;
  private userSubject: BehaviorSubject<Love2uUser | null> = new BehaviorSubject(null);
  public get user(): Observable<Love2uUser | null> {
    return this.userSubject.asObservable();
  }

  constructor(private configuration: AuthorizationConfiguration) {
    Log.logger = console;
    Log.level = Log.DEBUG;
  }

  public isAuthenticated(): Observable<boolean> {
    return this.getUser().pipe(map(u => !!u));
  }

  public getUser(): Observable<Love2uUser | null> {
    return concat(
      this.userSubject.pipe(
        take(1), 
        filter(u => !!u)
      ),
      this.getUserFromStorage().pipe(
        filter(u => !!u), 
        tap(u => this.userSubject.next(u))
      ),
      this.userSubject.asObservable());
  }

  public getAccessToken(): Observable<string> {
    return from(this.ensureUserManagerInitialized())
      .pipe(mergeMap(() => from(this.userManager.getUser())),
        map(user => user && user.access_token));
  }

  // We try to authenticate the user in three different ways:
  // 1) We try to see if we can authenticate the user silently. This happens
  //    when the user is already logged in on the IdP and is done using a hidden iframe
  //    on the client.
  // 2) We try to authenticate the user using a PopUp Window. This might fail if there is a
  //    Pop-Up blocker or the user has disabled PopUps.
  // 3) If the two methods above fail, we redirect the browser to the IdP to perform a traditional
  //    redirect flow.
  public async signIn(state: any): Promise<AuthenticationResult> {
    await this.ensureUserManagerInitialized();
    let user: User = null;
    try {
      user = await this.userManager.signinSilent(this.createArguments());
      if (user) {
        const profile = user.profile;
        const love2uUser: Love2uUser = { id: profile.sub, firstName: profile.given_name, lastName: profile.family_name };
        this.userSubject.next(love2uUser);
      }
      return this.success(state);
    } catch (silentError) {
      // User might not be authenticated, fallback to popup authentication
      console.log('Silent authentication error: ', silentError);

      try {
        if (this.popUpDisabled) {
          throw new Error('Popup disabled. Change \'authorize.service.ts:AuthorizeService.popupDisabled\' to false to enable it.');
        }
        user = await this.userManager.signinPopup(this.createArguments());
        if (user) {
          const profile = user.profile;
          const love2uUser: Love2uUser = { id: profile.sub, firstName: profile.given_name, lastName: profile.family_name };
          this.userSubject.next(love2uUser);
        }
        return this.success(state);
      } catch (popupError) {
        if (popupError.message === 'Popup window closed') {
          // The user explicitly cancelled the login action by closing an opened popup.
          return this.error('The user closed the window.');
        } else if (!this.popUpDisabled) {
          console.log('Popup authentication error: ', popupError);
        }

        // PopUps might be blocked by the user, fallback to redirect
        try {
          await this.userManager.signinRedirect(this.createArguments(state));
          return this.redirect();
        } catch (redirectError) {
          console.log('Redirect authentication error: ', redirectError);
          return this.error(redirectError);
        }
      }
    }
  }

  public async completeSignIn(url: string): Promise<AuthenticationResult> {
    try {
      await this.ensureUserManagerInitialized();
      const user = await this.userManager.signinCallback(url);
      if (user) {
        const profile = user.profile;
        const love2uUser: Love2uUser = { id: profile.sub, firstName: profile.given_name, lastName: profile.family_name };
        this.userSubject.next(love2uUser);
      }
      return this.success(user && user.state);
    } catch (error) {
      console.log('There was an error signing in: ', error);
      return this.error('There was an error signing in.');
    }
  }

  public async signOut(state: any): Promise<AuthenticationResult> {
    try {
      if (this.popUpDisabled) {
        throw new Error('Popup disabled. Change \'authorize.service.ts:AuthorizeService.popupDisabled\' to false to enable it.');
      }

      await this.ensureUserManagerInitialized();
      await this.userManager.signoutPopup(this.createArguments());
      this.userSubject.next(null);
      return this.success(state);
    } catch (popupSignOutError) {
      console.log('Popup signout error: ', popupSignOutError);
      try {
        await this.userManager.signoutRedirect(this.createArguments(state));
        return this.redirect();
      } catch (redirectSignOutError) {
        console.log('Redirect signout error: ', popupSignOutError);
        return this.error(redirectSignOutError);
      }
    }
  }

  public async completeSignOut(url: string): Promise<AuthenticationResult> {
    await this.ensureUserManagerInitialized();
    try {
      const state = await this.userManager.signoutCallback(url);
      this.userSubject.next(null);
      return this.success(state && state.data);
    } catch (error) {
      console.log(`There was an error trying to log out '${error}'.`);
      return this.error(error);
    }
  }

  private createArguments(state?: any): any {
    return { useReplaceToNavigate: true, data: state };
  }

  private error(message: string): AuthenticationResult {
    return { status: AuthenticationResultStatus.Fail, message };
  }

  private success(state: any): AuthenticationResult {
    return { status: AuthenticationResultStatus.Success, state };
  }

  private redirect(): AuthenticationResult {
    return { status: AuthenticationResultStatus.Redirect };
  }

  private async ensureUserManagerInitialized(): Promise<void> {
    if (this.userManager !== undefined) {
      return;
    }

    const response = await fetch(`${this.configuration.authority}/${AuthorizationPaths.CONFIGURATION_URL}/${this.configuration.clientId}`);
    if (!response.ok) {
      throw new Error(`Could not load settings for '${this.configuration.applicationName}'`);
    }

    const settings: any = await response.json();
    settings.automaticSilentRenew = true;
    settings.includeIdTokenInSilentRenew = true;
    this.userManager = new UserManager(settings);

    this.userManager.events.addUserSignedOut(async () => {
      await this.userManager.removeUser();
      this.userSubject.next(null);
    });
  }

  private getUserFromStorage(): Observable<Love2uUser> {
    return from(this.ensureUserManagerInitialized())
      .pipe(
        mergeMap(() => this.userManager.getUser()),
        map(u => u && u.profile));
  }
}
