import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { AuthorizationModule, AuthorizationConfiguration } from '@love2u/authorization';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@love2u/ui/material';
import { environment } from '../environments/environment';

const authSettings: AuthorizationConfiguration = {
  applicationName: environment.name, 
  authority: environment.identityProviderUrl
};

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([], { initialNavigation: 'enabled' }),
    AuthorizationModule.forRoot(authSettings),
    MaterialModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
