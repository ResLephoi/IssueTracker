import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Card } from '../../../models/card.model';
import { List } from '../../../models/list.model';
import { Board } from '../../../models/board.model';
import { BoardService } from '../../../core/services/board.service';
import { ListItemId } from '../../constants/constants';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DragDropModule],
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.less']
})
export class BoardComponent {
  board: Board = {
    id: '1',
    title: 'Project Tasks',
    lists: []
  };

  showEditDialog = false;
  editingCard: Card | null = null;
  currentList: List | null = null;
  cardForm: FormGroup;
  connectedLists: string[] = [];

  constructor(private fb: FormBuilder, private boardService: BoardService) {
    this.cardForm = this.fb.group({
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
      labels: ['']
    });
  }

  ngOnInit() {
    this.initializeBoard();
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
                  itemId: card.itemId
                };
              });
            }
            
            return list;
          });
          
          this.board = convertedBoard;
          
          this.connectedLists = this.board.lists.map(list => list.id);
        } else {
          console.error('Invalid board details response:', apiResponse);
          
          // Initialize with empty lists if not present in response
          this.board.lists = this.board.lists || [];
          this.connectedLists = this.board.lists.map(list => list.id);
        }
      },
      error: (err) => {
        console.error('Failed to load board details', err);
      }
    });
  }
  
  refreshBoard(): void {
    this.boardService.getBoardDetails(this.board.id).subscribe({
      next: (apiResponse: any) => {
        console.log('Board refresh response:', apiResponse);
        
        // Check if we have valid response with items
        if (!apiResponse || !apiResponse.items || !apiResponse.items.$values) {
          console.error('Invalid board details in refresh - missing items array:', apiResponse);
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
                itemId: card.itemId
              };
            });
          }
          
          return list;
        });
        
        this.board = convertedBoard;
        
        this.connectedLists = this.board.lists.map(list => list.id);
      },
      error: (err) => {
        console.error('Failed to refresh board', err);
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
          console.log('Card updated successfully', updatedCard);
        },
        error: (err) => {
          console.error('Failed to update card', err);
        }
      });
    }
  }

  openNewCardDialog(list: List) {
    this.currentList = list;
    this.editingCard = null;
    this.cardForm.reset();
    this.showEditDialog = true;
  }

  editCard(card: Card) {
    this.editingCard = card;
    this.cardForm.patchValue({
      title: card.title,
      description: card.description,
      labels: card.labels.join(', ')
    });
    this.showEditDialog = true;
  }

  deleteCard(list: List, card: Card) {       
      this.boardService.deleteCard(card.id).subscribe({
        next: () => {
          console.log('Card deleted successfully');
          this.refreshBoard();
        },
        error: (err) => {
          console.error('Failed to delete card', err);
        }
      });
  }
  
  
  saveCard() {
    if (this.cardForm.valid) {
      const formValue = this.cardForm.value;
      const labels = formValue.labels
        ? formValue.labels.split(',').map((label: string) => label.trim())
        : [];

      if (this.editingCard) {
        Object.assign(this.editingCard, {
          title: formValue.title,
          description: formValue.description,
          labels
        });
        
        this.boardService.updateCard(this.editingCard).subscribe({
          next: (updatedCard) => {
            console.log('Card updated successfully', updatedCard);
            this.closeDialog();
            this.refreshBoard();
          },
          error: (err) => {
            console.error('Failed to update card', err);
            this.closeDialog();
          }
        });
        return;
      } else if (this.currentList) {
        const payload = {
          title: formValue.title,
          description: formValue.description,
          labels,
          itemId: this.currentList.id
        };
        this.boardService.createCard(payload).subscribe({
          next: (createdCard) => {
            const newCard: Card = {
              id: createdCard.id || Date.now().toString(),
              title: createdCard.title,
              description: createdCard.description,
              labels: createdCard.labels
            };
            this.closeDialog();
            this.refreshBoard();
          },
          error: (err) => {
            this.closeDialog();
          }
        });
        return;
      }

      this.closeDialog();
    }
  }

  closeDialog() {
    this.showEditDialog = false;
    this.editingCard = null;
    this.currentList = null;
    this.cardForm.reset();
  }
}