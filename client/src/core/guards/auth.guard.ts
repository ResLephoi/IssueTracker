import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';

export const authGuard: CanMatchFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  return authService.currentUser$.pipe(
    map(user => {
      if (user) {
        return true;
      }
      
      // Not logged in, redirect to login page
      return router.createUrlTree(['/login']);
    })
  );
};

export const loggedInGuard: CanMatchFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  return authService.currentUser$.pipe(
    map(user => {
      if (user) {
        // User is already logged in, redirect to home
        return router.createUrlTree(['/board']);
      }
      
      return true;
    })
  );
};
