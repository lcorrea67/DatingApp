import { Component, OnInit, ViewChild } from '@angular/core';
import { UseExistingWebDriver } from 'protractor/built/driverProviders';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryImage, NgxGalleryOptions, NgxGalleryAnimation } from 'ngx-gallery';
import { AuthService } from 'src/app/_services/auth.service';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute,
              private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];  // taken from the routes.ts file for resolve configuration
    });

    this.route.queryParams.subscribe(params => {
      const selectedTab = params['tab'];  // taken from the routes.ts file for resolve configuration
      this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
      ];

    this.galleryImages = this.getImages();
  }

    getImages() {
      const imageUrls = [];
      for (let i = 0; i < this.user.photos.length; i++) {
        imageUrls.push({
          small: this.user.photos[i].url,
          medium: this.user.photos[i].url,
          big: this.user.photos[i].url,
          description: this.user.photos[i].description,
        });
      }

      return imageUrls;
    }

  sendLike(id: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, id).subscribe(data => {
      this.alertify.success('You have liked: ' + this.user.knownAs);
    }, error => {
      this.alertify.error(error);
    });
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }
  // loadUser() {                    // e.g. members/4, the + forces the string id to be a number
  //  this.userService.getUser(+this.route.snapshot.params['id']).subscribe((user: User) => {
  //    this.user = user;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

}
