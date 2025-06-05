import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Card } from '../../models/card.model';
import { Board } from '../../models/board.model';
import { List } from '../../models/list.model';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private apiUrl = 'https://localhost:7069/api';

  constructor(private http: HttpClient) {}

  getCards(): Observable<Card[]> {
    return this.http.get<Card[]>(`${this.apiUrl}/card/GetAllCards`);
  }

  createCard(payload: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/card`, payload);
  }

  updateCard(card: Card): Observable<Card> {
    return this.http.put<Card>(`${this.apiUrl}/card/${card.id}`, card);
  }

  deleteCard(cardId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/card/${cardId}`);
  }

  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(`${this.apiUrl}/board`);
  }

  getLists(): Observable<List[]> {
    return this.http.get<List[]>(`${this.apiUrl}/list`);
  }
  
  getBoardDetails(boardId: string): Observable<any> {
    const url = `${this.apiUrl}/board/${boardId}/details`;
    console.log('Fetching board details from URL:', url);
    
    return this.http.get<any>(url)
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