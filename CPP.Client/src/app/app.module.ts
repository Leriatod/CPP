import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";

import { AppComponent } from "./app.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { CarFormComponent } from "./car-form/car-form.component";
import { CustomFormsModule } from "ng2-validation";

@NgModule({
  declarations: [AppComponent, NavbarComponent, CarFormComponent],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    CustomFormsModule,
    RouterModule.forRoot([{ path: "", component: CarFormComponent }]),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
