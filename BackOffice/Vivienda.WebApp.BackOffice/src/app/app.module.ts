import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AgenteRegisterComponent } from './account/register/agente-register/agente-register.component';
import { SigninComponent } from './account/auth/signin/signin.component';
import { SignupComponent } from './account/auth/signup/signup.component';
import { BrokerRegisterComponent } from './account/register/broker-register/broker-register.component';

@NgModule({
  declarations: [
    AppComponent,
    AgenteRegisterComponent,
    SigninComponent,
    SignupComponent,
    BrokerRegisterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
