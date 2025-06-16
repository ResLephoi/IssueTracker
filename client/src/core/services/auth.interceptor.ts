/**
 * @deprecated This class-based interceptor is not actively used in the application.
 * The functional interceptor in main.ts is used instead.
 * 
 * This file is kept for reference and can be safely deleted if not needed.
 * Token refresh functionality is now implemented directly in the functional
 * interceptor in main.ts and the AuthService.
 */

import { Injectable } from '@angular/core';
import { 
  HttpRequest, 
  HttpHandler, 
  HttpEvent, 
  HttpInterceptor, 
  HttpErrorResponse 
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take, finalize } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Get the auth token from the service
    const currentUser = this.authService.currentUserValue;
    
    if (currentUser && currentUser.token) {
      request = this.addToken(request, currentUser.token);
    }

    return next.handle(request).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          // Auto logout if 401 response returned from API
          this.authService.logout();
          // Location reload is optional and can be removed if you prefer to handle this differently
          location.reload();
        }
        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
}
