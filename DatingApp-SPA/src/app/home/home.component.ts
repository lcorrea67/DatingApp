import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"]
})
export class HomeComponent implements OnInit {
  registerMode = false;
  values: any;

  constructor(private http: HttpClient) {}

  ngOnInit() {
  }

  registerToggle() {
    this.registerMode = true;
  }

  // the cancel button in the register component will call this method
  cancelRegisterMode(registerMode) {
    this.registerMode = registerMode;
  }

}
