import { Component, ViewChild, OnInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Card } from '../../../models/card.model';
import { List } from '../../../models/list.model';
import { Board } from '../../../models/board.model';
import { User } from '../../../models/user.model';
import { BoardService } from '../../../core/services/board.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { CardComponent } from '../card/card.component';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropModule, CardComponent],
  templateUrl: './board.component.html',
  styleUrls: ['../../../global_styles.css', './board.component.less'],
  encapsulation: ViewEncapsulation.None
})
export class BoardComponent implements OnInit {
  @ViewChild(CardComponent) cardComponent!: CardComponent;

  board: Board = {
    id: '1',
    title: 'Project Tasks',
    lists: []
  };

  showEditDialog = false;
  editingCard: Card | null = null;
  currentList: List | null = null;
  connectedLists: string[] = [];
  users: User[] = [];
  selectedFilter: string = '';
  searchTerm: string = '';
  filteredCards: { [listId: string]: Card[] } = {};
  showingFilteredView: boolean = false;
  isRefreshing = false;

  constructor(
    private boardService: BoardService, 
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.initializeBoard();
    this.loadUsers();    
  }
  
  loadUsers() {
    this.authService.getUsers().subscribe({
      next: (users: User[]) => {
        this.users = users || [];        
      },
      error: (err) => {        
        // Show more specific error messages
        if (err.status === 0) {
          this.toastService.error('Cannot connect to server. Please check if the API server is running on https://localhost:7069');
        } else if (err.status === 401) {
          this.toastService.error('Authentication failed. Please log in again.');
        } else if (err.status === 404) {
          this.toastService.error('Users endpoint not found. Please check API configuration.');
        } else {
          this.toastService.warning('Failed to load users for assignment. Some features may be limited.');
        }
      }
    });
  }

  initializeBoard(): void {
    const boardId = this.board.id;
    this.boardService.getBoardDetails(boardId).subscribe({
      next: (apiResponse: any) => {
        
        // Convert API response format to our Board model format
        if (apiResponse && apiResponse.items && apiResponse.items.$values) {
          const convertedBoard: Board = {
            id: apiResponse.id.toString(),
            title: apiResponse.title,
            lists: []
          };
          
          convertedBoard.lists = apiResponse.items.$values.map((item: any) => {
            const list: List = {
              id: item.id.toString(), 
              title: item.title,
              cards: []
            };
            
            // Add cards if they exist
            if (item.cards && item.cards.$values) {
              list.cards = item.cards.$values.map((card: any) => {
                return {
                  id: card.id.toString(),
                  title: card.title,
                  description: card.description,
                  labels: card.labels && card.labels.$values ? card.labels.$values : [],
                  itemId: card.itemId,
                  assignedToUserId: card.assignedToUserId ?? undefined
                };
              });
            }
            
            return list;
          });
          
          this.board = convertedBoard;
          
          this.connectedLists = this.board.lists.map(list => list.id);
        } else {
          // Initialize with empty lists if not present in response
          this.board.lists = this.board.lists || [];
          this.connectedLists = this.board.lists.map(list => list.id);
        }
      },
      error: (err) => {
      }
    });
  }
  
  refreshBoard(): void {
    if (this.isRefreshing) {
      return; // Prevent multiple simultaneous refresh operations
    }
    
    this.isRefreshing = true;
    this.boardService.getBoardDetails(this.board.id).subscribe({
      next: (apiResponse: any) => {
        // Check if we have valid response with items
        if (!apiResponse || !apiResponse.items || !apiResponse.items.$values) {
          return;
        }
        
        const convertedBoard: Board = {
          id: apiResponse.id.toString(),
          title: apiResponse.title,
          lists: []
        };
        
        convertedBoard.lists = apiResponse.items.$values.map((item: any) => {
          const list: List = {
            id: item.id.toString(), 
            title: item.title,
            cards: []
          };
          
          if (item.cards && item.cards.$values) {
            list.cards = item.cards.$values.map((card: any) => {
              return {
                id: card.id.toString(),
                title: card.title,
                description: card.description,
                labels: card.labels && card.labels.$values ? card.labels.$values : [],
                itemId: card.itemId,
                assignedToUserId: card.assignedToUserId ?? undefined
              };
            });
          }
          
          return list;
        });
        
        this.board = convertedBoard;
        
        this.connectedLists = this.board.lists.map(list => list.id);
        
        // Reapply filters if they were active
        if (this.showingFilteredView) {
          this.applyFilters();
        }
        
        this.isRefreshing = false;
      },
      error: (err) => {
        this.toastService.error('Failed to refresh board data. Please refresh the page.');
        this.isRefreshing = false;
      }
    });
  }

  drop(event: CdkDragDrop<Card[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const movedCard = event.previousContainer.data[event.previousIndex];
      movedCard.itemId = Number(event.container.id);
      
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      
      this.boardService.updateCard(movedCard).subscribe({
        next: (updatedCard) => {
          const cardTitle = updatedCard?.title || movedCard?.title || 'Card';
          this.toastService.success(`Card "${cardTitle}" moved successfully!`);
        },
        error: (err) => {
          this.toastService.error('Failed to move card. Please try again.');
          // Revert the move on error
          transferArrayItem(
            event.container.data,
            event.previousContainer.data,
            event.currentIndex,
            event.previousIndex
          );
        }
      });
    }
  }

  openNewCardDialog(list: List) {
    this.currentList = list;
    this.editingCard = null;
    this.showEditDialog = true;
  }

  editCard(card: Card) {
    this.editingCard = card;
    this.showEditDialog = true;
  }

  deleteCard(list: List, card: Card) {
    // Use the card component's delete functionality with confirmation
    this.cardComponent?.deleteCard(card);
  }

  onCardSaved() {
    this.refreshBoard();
  }

  onCardDeleted() {
    this.refreshBoard();
  }

  filterByUser(userId: string) {
    this.selectedFilter = userId;
    this.applyFilters();
  }

  searchCards(searchTerm: string) {
    this.searchTerm = searchTerm;
    this.applyFilters();
  }

  applyFilters() {
    if (!this.selectedFilter && !this.searchTerm) {
      this.showingFilteredView = false;
      this.filteredCards = {};
      return;
    }
    
    this.showingFilteredView = true;
    this.filteredCards = {};
    
    const userIdNumber = this.selectedFilter ? Number(this.selectedFilter) : null;
    const searchTermLower = this.searchTerm.toLowerCase();
    
    this.board.lists.forEach(list => {
      const filteredListCards = list.cards.filter(card => {
        // Apply user filter
        let passesUserFilter = true;
        if (userIdNumber) {
          passesUserFilter = card.assignedToUserId === userIdNumber;
        }
        
        // Apply search filter
        let passesSearchFilter = true;
        if (this.searchTerm) {
          passesSearchFilter = 
            card.title.toLowerCase().includes(searchTermLower) ||
            card.description.toLowerCase().includes(searchTermLower) ||
            (card.labels && card.labels.some(label => label.toLowerCase().includes(searchTermLower)));
        }
        
        return passesUserFilter && passesSearchFilter;
      });
      this.filteredCards[list.id] = filteredListCards || [];
    });
  }

  clearFilters() {
    this.selectedFilter = '';
    this.searchTerm = '';
    this.applyFilters();
  }
  
  getUserById(userId: number | string | undefined): User | undefined {
    if (userId === null || userId === undefined || !this.users || this.users.length === 0) {
      return undefined;
    }
    
    const userIdNum = Number(userId);
   
    const user = this.users.find(u => {
      const numId = Number(u.id);
      return numId === userIdNum;
    });
    return user;
  }

  closeDialog() {
    this.showEditDialog = false;
    this.editingCard = null;
    this.currentList = null;
  }
}