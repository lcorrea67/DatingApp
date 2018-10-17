import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  // the input allows to accept a value from another component (e.g. HomeComponent)
  // [@]Input() valuesFromHome: any;

  // output properties emit events to the parent component
  @Output() cancelRegister = new EventEmitter();

  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {}
  register() {
      this.authService.register(this.model).subscribe(() => {
        console.log('registration successful');
      }, error => {
        console.log(error);
      });
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }
}
