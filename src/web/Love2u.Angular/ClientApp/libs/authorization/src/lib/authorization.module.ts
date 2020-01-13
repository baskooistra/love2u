import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { AuthorizationConfiguration, AuthorizationPaths } from './authorization.constants';
import { AuthorizationService } from './authorization.service';
import { RouterModule, Routes } from '@angular/router';
import { LogoutComponent } from './logout/logout.component';

const routes: Routes = [
  { path: AuthorizationPaths.LOGIN, component: LoginComponent },
  { path: AuthorizationPaths.LOGIN_FAILED, component: LoginComponent },
  { path: AuthorizationPaths.LOGIN_CALLBACK, component: LoginComponent },
  { path: AuthorizationPaths.LOGOUT, component: LogoutComponent },
  { path: AuthorizationPaths.LOGGED_OUT, component: LogoutComponent },
  { path: AuthorizationPaths.LOGOUT_CALLBACK, component: LogoutComponent }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  declarations: [LoginComponent, LogoutComponent]
})
export class AuthorizationModule {
  static forRoot(configuration: AuthorizationConfiguration): ModuleWithProviders {
    return {
      ngModule: AuthorizationModule,
      providers: [{ 
        provide: AuthorizationService,
        useFactory() {
          return new AuthorizationService(configuration);
        }
      }]
    }
  }
}
