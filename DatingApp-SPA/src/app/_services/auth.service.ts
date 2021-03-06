import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png'); // shows on the Nav bar when the user changes their main photo
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login (model: any) {
    // the response form the server needs to be 'piped' so angular can work with it
    // response will be returned as
    // {
    //    "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9
    //                .eyJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiJqb2huIiwibmJmIjoxNTM5NDU4MDcyLCJleHAiOjE1Mzk1NDQ0NzIsImlhdCI6MTUzOTQ1ODA3Mn0
    //                .N_WPHaZzrSJmQsNw_bLQ2Z788SgicbUX_9iX1MRJCoFWxi7xg9AmxpGgsT2a3D4AxQxHgd50SrrHO9bF3BFJMg"
    // }
    return this.http.post(this.baseUrl + 'login', model).pipe(
                      map((response: any) => {
                        const user = response;
                        if (user) {
                          localStorage.setItem('token', user.token);
                          localStorage.setItem('user', JSON.stringify(user.user));

                          this.decodedToken = this.jwtHelper.decodeToken(user.token);
                          this.currentUser = user.user;
                          this.changeMemberPhoto(this.currentUser.photoUrl);

                          console.log(this.decodedToken);
                        }
                      })
                    );
  }

  register(user: User) {
    // the poost method returns an Observable so we need to suscribe to it from the register compoent
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }


}
