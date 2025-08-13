import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors, HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NavigationBarComponent } from './app/navigation-bar.component';
import { routes } from './app/app.routes';
import { inject } from '@angular/core';
import { AuthService } from './core/services/auth.service';
import { catchError, throwError } from 'rxjs';

// Functional interceptor approach
const authInterceptorFn: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const currentUser = authService.currentUserValue;
  
  // Skip auth header for login endpoint
  if (req.url.includes('/login')) {
    return next(req);
  }
  
  if (currentUser && currentUser.token) {
    // Check if user is still logged in (this will handle token expiration)
    if (!authService.isLoggedIn()) {
      console.log('Token expired, redirecting to login');
      return next(req);
    }
    
    // Clone the request and add auth header
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${currentUser.token}`
      }
    });
    console.log('Adding auth header to request:', req.url);
    
    return next(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          console.log('401 error received, logging out user');
          authService.logout();
          // You can add router navigation here if needed
          // const router = inject(Router);
          // router.navigate(['/login']);
        }
        return throwError(() => error);
      })
    );
  }
  
  console.log('No auth token available for request:', req.url);
  return next(req);
};

bootstrapApplication(NavigationBarComponent, {
  providers: [
    provideHttpClient(withInterceptors([authInterceptorFn])),
    provideRouter(routes)
  ]
});