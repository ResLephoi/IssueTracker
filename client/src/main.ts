import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors, HttpInterceptorFn } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NavigationBarComponent } from './app/navigation-bar.component';
import { routes } from './app/app.routes';
import { inject } from '@angular/core';
import { AuthService } from './core/services/auth.service';

// Functional interceptor approach
const authInterceptorFn: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const currentUser = authService.currentUserValue;
  
  // Skip auth header for login endpoint
  if (req.url.includes('/login')) {
    return next(req);
  }
  
  if (currentUser && currentUser.token) {
    // Clone the request and add auth header
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${currentUser.token}`
      }
    });
    console.log('Adding auth header to request:', req.url);
    return next(authReq);
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