import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Card } from '../../models/card.model';
import { Board } from '../../models/board.model';
import { List } from '../../models/list.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private apiUrl = 'https://localhost:7069/api';

  constructor(
    private http: HttpClient,
    private authService: AuthService) {}

  getCards(): Observable<Card[]> {
    return this.http.get<Card[]>(`${this.apiUrl}/card/GetAllCards`, { headers: this.authService.getAuthHeaders() });
  }

  createCard(payload: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/card`, payload, { headers: this.authService.getAuthHeaders() });
  }

  updateCard(card: Card): Observable<Card> {
    return this.http.put<Card>(`${this.apiUrl}/card/${card.id}`, card, { headers: this.authService.getAuthHeaders() });
  }

  deleteCard(cardId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/card/${cardId}`, { headers: this.authService.getAuthHeaders() });
  }

  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(`${this.apiUrl}/board`, { headers: this.authService.getAuthHeaders() });
  }

  getLists(): Observable<List[]> {
    return this.http.get<List[]>(`${this.apiUrl}/list`, { headers: this.authService.getAuthHeaders() });
  }
  
  getBoardDetails(boardId: string): Observable<any> {
    const url = `${this.apiUrl}/board/${boardId}/details`;
    console.log('Fetching board details from URL:', url);
    
    return this.http.get<any>(url, { headers: this.authService.getAuthHeaders() })
      .pipe(
        map((response: any) => {
          console.log('Raw board details response:', response);
          
          // If API returns data wrapped in a data property, unwrap it
          if (response.data && typeof response.data === 'object') {
            console.log('Unwrapping data property from response');
            return response.data;
          }
          
          return response;
        })
      );
  }
}