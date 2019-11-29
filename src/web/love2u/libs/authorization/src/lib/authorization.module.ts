import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { AuthorizationConfiguration } from './authorization.constants';
import { AuthorizationService } from './authorization.service';

@NgModule({
  imports: [
    CommonModule    
  ],
  declarations: [LoginComponent]
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
