import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { debounceTime, distinctUntilChanged, map } from "rxjs/operators";

import { CarService } from "../car.service";
import { Car } from "../models/car";
import { CarFeatureCategories } from "../models/car-feature-categories";

@Component({
  selector: "car-form",
  templateUrl: "./car-form.component.html",
  styleUrls: ["./car-form.component.css"],
})
export class CarFormComponent implements OnInit {
  isLoading = true;

  searchProducer$: (text$: Observable<string>) => Observable<string[]>;
  searchModel$: (text$: Observable<string>) => Observable<string[]>;
  searchBody$: (text$: Observable<string>) => Observable<string[]>;
  searchDrive$: (text$: Observable<string>) => Observable<string[]>;
  searchTransmission$: (text$: Observable<string>) => Observable<string[]>;
  searchFuel$: (text$: Observable<string>) => Observable<string[]>;

  featureCategories: CarFeatureCategories = {
    producers: [],
    models: [],
    bodies: [],
    drives: [],
    transmissions: [],
    fuels: [],
  };

  car: Car = {
    producer: "",
    model: "",
    body: "",
    drive: "",
    transmission: "",
    fuel: "",
    engine: null,
    horsepower: null,
    distance: null,
    year: null,
  };

  price: number = 0.0;

  constructor(private carService: CarService) {}

  ngOnInit() {
    this.carService.getCarFeatureCategories().subscribe((featureCategories) => {
      this.featureCategories = featureCategories;
      this.searchProducer$ = this.createSearchPipe(featureCategories.producers);
      this.searchModel$ = this.createSearchPipe(featureCategories.models);
      this.searchBody$ = this.createSearchPipe(featureCategories.bodies);
      this.searchDrive$ = this.createSearchPipe(featureCategories.drives);
      this.searchTransmission$ = this.createSearchPipe(
        featureCategories.transmissions
      );
      this.searchFuel$ = this.createSearchPipe(this.featureCategories.fuels);

      this.isLoading = false;
    });
  }

  predictPrice() {
    this.carService.predictPriceForCar(this.car).subscribe((price) => {
      this.price = price;
    });
  }

  createSearchPipe(categories: string[]) {
    return (text$: Observable<string>) =>
      text$.pipe(
        debounceTime(200),
        distinctUntilChanged(),
        map((template: string) =>
          this.filterCategoriesByTemplate(categories, template).slice(0, 10)
        )
      );
  }

  filterCategoriesByTemplate(categories: string[], template: string) {
    template = template.toLowerCase();

    const filteredCategories = categories.filter((category) => {
      category = category.toLowerCase();
      const hasTemplate = category.indexOf(template) > -1;
      return hasTemplate;
    });

    return filteredCategories;
  }
}
