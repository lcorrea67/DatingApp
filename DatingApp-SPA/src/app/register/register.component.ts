import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // the input allows to accept a value from another component (e.g. HomeComponent)
  @Input() valuesFromHome: any;
  model: any = {};

  constructor() { }

  ngOnInit() {
  }

  register() {
      console.log(this.model);
  }

  cancel() {
    console.log('cancelled');
  }
}
