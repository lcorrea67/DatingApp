import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

// ubhectible indicates it can be injected using Dependency Injection (DI)
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    // interface HttpInterceptor request and concrete intercept method 
    // take a request parameter and a next parameter of type HttpHandler
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // we need to handle the request and then use the rxjs pipe
        return next.handle(req).pipe(
            catchError(error => {
                if (error instanceof HttpErrorResponse) {
                    if (error.status === 401) {
                        return throwError(error.statusText);
                    }

                    // get the header from the Http pipline
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        console.log(applicationError);
                        // throwError is a type of observable
                        return throwError(applicationError);
                    }

                    const serverError = error.error;
                    let modelStateErrors = '';
                    if (serverError && typeof serverError === 'object') {
                        for (const key in serverError) {
                            if (serverError[key]) {
                                modelStateErrors += serverError[key] + '\n';
                            }
                        }
                    }

                    return throwError(modelStateErrors || serverError || 'Server Error');
                }
            })
        );
    }
}

// befofe we can use rht ErrorInterceptor, we  need to create an ErrorInterceptorProvider so out module can import it
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
}; // support other interceptors. ErrorInterceptor will be added to the array of existing interceptor's

