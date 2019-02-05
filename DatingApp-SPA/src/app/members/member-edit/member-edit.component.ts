import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;  // gives access the HTML NgForm element within the 'code behind'
  user: User;
  photoUrl: string;

  @HostListener('window:beforeunload', ['$event'])  // checks if the browser window is being closed
  unloadNotification($event: any) { // if the browser window is closed then this pops the message
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  // tslint:disable-next-line:max-line-length
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private userSrvice: UserService, private authService: AuthService) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  updateUser() {
    //   this.authService.decodedToken.nameid gives the Id of the logged in user
    this.userSrvice.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('Profile updated successfuly');
      this.editForm.reset(this.user); // reset will set the form back to not dirty and will also clear it
                                    // adding the paramter this.user will fill the form input fields ngForm
    }, error => {
      this.alertify.error(error);
    });
  }

    updateMainPhoto(photoUrl) {
      this.user.photoUrl = photoUrl;
    }
}
