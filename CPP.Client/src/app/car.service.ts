import { CarFeatureCategories } from "./models/car-feature-categories";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Car } from "./models/car";

@Injectable({
  providedIn: "root",
})
export class CarService {
  baseUrl = "/api/cars";

  constructor(private httpClient: HttpClient) {}

  getCarFeatureCategories() {
    return this.httpClient.get<CarFeatureCategories>(
      `${this.baseUrl}/feature-categories`
    );
  }

  predictPriceForCar(car: Car) {
    console.log(car);
    return this.httpClient.post<number>(`${this.baseUrl}/predict`, car);
  }
}
