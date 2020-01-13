import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { AuthorizationModule, AuthorizationConfiguration } from '@love2u/authorization';
import { AppComponent } from './app.component';
import { RouterModule, Routes } from '@angular/router';
import { MaterialModule } from '@love2u/ui/material';
import { environment } from '../environments/environment';

const authSettings: AuthorizationConfiguration = {
  applicationName: environment.name, 
  authority: environment.identityProviderUrl,
  clientId: "Love2uAngular"
};

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'home', },
  { path: 'home', component: AppComponent }
];

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule.forRoot(routes, { initialNavigation: 'enabled' }),
    AuthorizationModule.forRoot(authSettings),
    MaterialModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
