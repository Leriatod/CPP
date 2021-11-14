import { Component, OnInit } from "@angular/core";
import { CarService } from "../car.service";
import { Car } from "../models/car";
import { CarFeatureCategories } from "../models/car-feature-categories";

@Component({
  selector: "car-form",
  templateUrl: "./car-form.component.html",
  styleUrls: ["./car-form.component.css"],
})
export class CarFormComponent implements OnInit {
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
    });
  }

  onProducerChange(producer: string) {
    this.car.producer = producer;
    this.car.model = "";
  }

  predictPrice() {
    this.carService.predictPriceForCar(this.car).subscribe((price) => {
      this.price = price;
    });
  }
}
