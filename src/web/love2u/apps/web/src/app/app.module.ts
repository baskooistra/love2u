import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AuthorizationModule, AuthorizationConfiguration } from '@love2u/authorization';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    RouterModule.forRoot([], { initialNavigation: 'enabled' }),
    AuthorizationModule.forRoot({ applicationName: environment.name, authority: environment.identityProviderUrl })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
