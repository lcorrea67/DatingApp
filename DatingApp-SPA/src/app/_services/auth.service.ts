import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = "http://localhost:5000/api/auth/";

  constructor(private http: HttpClient) { }

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
                        }
                      })
                    );
  }

}
