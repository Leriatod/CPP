import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule } from "@angular/router";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { CustomFormsModule } from "ng2-validation";

import { AppComponent } from "./app.component";
import { CarFormComponent } from "./car-form/car-form.component";
import { NavbarComponent } from "./navbar/navbar.component";

@NgModule({
  declarations: [AppComponent, NavbarComponent, CarFormComponent],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    NgbModule,
    CustomFormsModule,
    RouterModule.forRoot([{ path: "", component: CarFormComponent }]),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
