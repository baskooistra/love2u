import { Component } from '@angular/core';
import { AuthorizationService, AuthorizationPaths } from '@love2u/authorization';
import { Router, ActivatedRoute } from '@angular/router';
import { tap, take } from 'rxjs/operators';

@Component({
  selector: 'love2u-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  title = 'Love2u';

  constructor(private authorizationService: AuthorizationService, private router: Router, private activatedRoute: ActivatedRoute) {

  }

  handleLogin() {
    this.authorizationService.isAuthenticated().pipe(
      take(1),
      tap(authenticated => this.handleAuthorization(authenticated))).subscribe();
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
