import { Routes } from '@angular/router';
import { BoardComponent } from './features/board/board.component';
import { LoginComponent } from './features/auth/login.component';
import { authGuard, loggedInGuard } from '../core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    canMatch: [loggedInGuard]
  },
  {
    path: 'board',
    component: BoardComponent,
    canMatch: [authGuard]
  },
  { 
    path: '', 
    redirectTo: 'board', 
    pathMatch: 'full' 
  },
  { 
    path: '**', 
    redirectTo: 'board' 
  }
];
