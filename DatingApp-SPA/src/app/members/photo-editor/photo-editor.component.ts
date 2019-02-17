import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';


@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
// as this is a child component, we need to pass data from the parent
@Input() photos: Photo[];
@Output() getMemberPhotoChange = new EventEmitter<string>();

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024   // 10MB
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    // this will show the photo after it is successfully oploaded
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
          const res: Photo = JSON.parse(response);
          const photo = {
            id: res.id,
            url: res.url,
            dateAdded: res.dateAdded,
            description: res.description,
            isMain: res.isMain
          };

          this.photos.push(photo);
          if (photo.isMain) {
            this.authService.changeMemberPhoto(photo.url);
            this.currentMain = this.photos.filter(p => p.isMain === true)[0];
            this.currentMain.isMain = false;          }
      }
    };
  }

  setMainPhoto(photo: Photo) {
      this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMain = this.photos.filter( p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      this.authService.changeMemberPhoto(photo.url);

      // to change the main photo, we will need to send a message to another component, the member edit component
      // we need to use output properties, we will need to often update a parent component from a child compnent
      // (getMemberPhotoChange)="updateMainPhoto($event)"
      this.getMemberPhotoChange.emit(photo.url);

    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto(id: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {

        // splice removes elements from an array
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      this.alertify.success('Photo has been deleted');
      }, error => {
        this.alertify.error('Failed to delete photo');
      });
    });
  }
}
