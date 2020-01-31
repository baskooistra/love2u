import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay, take, tap } from 'rxjs/operators';
import { AuthorizationService, AuthorizationPaths } from '@love2u/authorization';
import { Love2uUser } from 'libs/authorization/src/lib/authorization.service';
import { Router } from '@angular/router';

@Component({
  selector: 'love2u-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LayoutComponent implements OnInit {
  loggedInUser$: Observable<Love2uUser | null>

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(private breakpointObserver: BreakpointObserver, private router: Router, private authorizationsService: AuthorizationService) {
  }

  ngOnInit() {
    this.loggedInUser$ = this.authorizationsService.user;
  }

  handleLogin() {
    this.authorizationsService.isAuthenticated().pipe(
      take(1),
      tap(authenticated => this.handleAuthorization(authenticated))).subscribe();
  }

  handleLogout() {
    
  }

  private handleAuthorization(authenticated: boolean) {
    if (!authenticated) {
      this.router.navigate([AuthorizationPaths.LOGIN], {
        queryParams: {
          returnUrl: this.router.routerState.snapshot.url
        }
      });
    }
  }
}
