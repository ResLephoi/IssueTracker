// filepath: /IssueTracker/IssueTracker/client/src/app/features/board/board.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Board, List, Card } from '../../../models/board.model';
import { BoardService } from '../../core/services/board.service';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, DragDropModule, ReactiveFormsModule],
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.less']
})
export class BoardComponent implements OnInit {
  board: Board = {
    id: '1',
    title: 'Project Tasks',
    lists: [
      {
        id: 'todo',
        title: 'To Do',
        cards: [
          { id: '1', title: 'Implement login', description: 'Add user authentication', labels: ['feature'] },
          { id: '2', title: 'Design UI', description: 'Create mockups', labels: ['design'] }
        ]
      },
      {
        id: 'inProgress',
        title: 'In Progress',
        cards: []
      },
      {
        id: 'done',
        title: 'Done',
        cards: []
      }
    ]
  };

  showEditDialog = false;
  editingCard: Card | null = null;
  currentList: List | null = null;
  cardForm: FormGroup;
  connectedLists: string[];

  constructor(private fb: FormBuilder,
    private boardService: BoardService) {
    this.cardForm = this.fb.group({
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
      labels: ['']
    });
    this.connectedLists = this.board.lists.map(list => list.id);
  }

  ngOnInit(): void {}

  drop(event: CdkDragDrop<Card[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
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
    this.boardService.deleteCard(card.id).subscribe(() => {
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
      } else if (this.currentList) {
        const newCard: Card = {
          id: Date.now().toString(),
          title: formValue.title,
          description: formValue.description,
          labels
        };
        this.currentList.cards.push(newCard);
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