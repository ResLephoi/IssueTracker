import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { User } from '../models/user.model';
import { ToastComponent } from './shared/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet, 
    RouterModule,
    ToastComponent
  ],
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.less']
})
export class NavigationBarComponent implements OnInit {
  currentUser: User | null = null;
  showUserMenu: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      
      // Close menus on auth state change
      this.showUserMenu = false;
    });
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.showUserMenu = false;
  }
}
