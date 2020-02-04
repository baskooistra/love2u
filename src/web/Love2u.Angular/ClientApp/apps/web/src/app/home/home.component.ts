import { Component } from '@angular/core';
import { AuthorizationService, AuthorizationPaths } from '@love2u/authorization';
import { Router, ActivatedRoute } from '@angular/router';
import { tap, take, mergeMap } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'love2u-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  title = 'Love2u';

  constructor(private authorizationService: AuthorizationService, private router: Router, private activatedRoute: ActivatedRoute, private http: HttpClient) {

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

  handleGetStartedClick() {
    this.authorizationService.getAccessToken().pipe(
      mergeMap((token: string) => {
        const headers = new HttpHeaders({
          'Authorization': 'Bearer ' + token
        });
        return this.http.get("https://localhost:44352/api/user/profile/userprofile", { headers: headers });
      })).subscribe(result => {
        console.log(result);
    });
  }
}
