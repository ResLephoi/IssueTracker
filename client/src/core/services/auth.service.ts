import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { User, LoginRequest, LoginResponse } from '../../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private apiUrl = 'https://localhost:7069/api/Auth';
  
  constructor(private http: HttpClient) {
    // Check if user is already logged in from localStorage
    this.loadStoredUser();
  }

  private loadStoredUser(): void {
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      try {
        const user: User = JSON.parse(storedUser);
        this.currentUserSubject.next(user);
      } catch (error) {
        localStorage.removeItem('currentUser');
      }
    }
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  login(loginRequest: LoginRequest): Observable<User> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginRequest)
      .pipe(
        map(response => {
          if (!response.success) {
            throw new Error(response.message || 'Login failed');
          }
          
          // Create user object from the API response
          const user: User = {
            id: crypto.randomUUID(),
            username: response.username,
            token: response.token
          };
          
          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);
          return user;
        }),
        catchError(error => {
          // Handle HTTP errors
          if (error.status === 401) {
            return throwError(() => new Error('Invalid username or password'));
          } else if (error.status === 400) {
            return throwError(() => new Error(error.error?.message || 'Invalid request'));
          } else {
            return throwError(() => new Error('An error occurred. Please try again later.'));
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}
