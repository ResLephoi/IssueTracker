import { Component, Input, Output, EventEmitter, OnInit, OnChanges, ViewEncapsulation, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Card } from '../../../models/card.model';
import { List } from '../../../models/list.model';
import { User } from '../../../models/user.model';
import { BoardService } from '../../../core/services/board.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './card.component.html',
  styleUrls: ['../../../global_styles.css', './card.component.less'],
  encapsulation: ViewEncapsulation.None
})
export class CardComponent implements OnInit, OnChanges {
  @Input() showEditDialog = false;
  @Input() editingCard: Card | null = null;
  @Input() currentList: List | null = null;
  @Input() users: User[] = [];
  
  @Output() closeDialogEvent = new EventEmitter<void>();
  @Output() cardSavedEvent = new EventEmitter<void>();
  @Output() cardDeletedEvent = new EventEmitter<void>();

  cardForm: FormGroup;
  showDeleteConfirmation = false;
  cardToDelete: Card | null = null;

  constructor(
    private fb: FormBuilder,
    private boardService: BoardService,
    private toastService: ToastService
  ) {
    this.cardForm = this.fb.group({
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
      labels: [''],
      assignedToUserId: ['']
    });
  }

  ngOnInit() {
    // Initialize form when component loads
    if (this.editingCard) {
      this.populateForm();
    }
  }

  ngOnChanges() {
    // Handle changes to inputs
    if (this.editingCard) {
      this.populateForm();
    } else if (this.showEditDialog && !this.editingCard) {
      // New card - reset form
      this.cardForm.reset();
    }
  }

  @HostListener('document:keydown.escape', ['$event'])
  onEscapeKey(event: KeyboardEvent) {
    if (this.showEditDialog) {
      this.closeDialog();
    } else if (this.showDeleteConfirmation) {
      this.cancelDelete();
    }
  }

  private populateForm() {
    if (this.editingCard) {
      this.cardForm.patchValue({
        title: this.editingCard.title,
        description: this.editingCard.description,
        labels: this.editingCard.labels.join(', '),
        assignedToUserId: this.editingCard.assignedToUserId ?? ''
      });
    }
  }

  saveCard() {
    if (this.cardForm.valid) {
      const formValue = this.cardForm.value;
      const labels = formValue.labels
        ? formValue.labels.split(',').map((label: string) => label.trim())
        : [];

      if (this.editingCard) {
        // Update existing card
        Object.assign(this.editingCard, {
          title: formValue.title,
          description: formValue.description,
          labels,
          assignedUserId: formValue.assignedToUserId ?? null
        });
        
        this.boardService.updateCard(this.editingCard).subscribe({
          next: (updatedCard) => {
            const cardTitle = updatedCard?.title || this.editingCard?.title || 'Card';
            this.toastService.success(`Card "${cardTitle}" updated successfully!`);
            this.closeDialog();
            this.cardSavedEvent.emit();
          },
          error: (err) => {
            this.toastService.error('Failed to update card. Please try again.');
            this.closeDialog();
          }
        });
      } else if (this.currentList) {
        // Create new card
        const payload = {
          title: formValue.title,
          description: formValue.description,
          labels,
          itemId: this.currentList.id,
          assignedToUserId: formValue.assignedToUserId ?? null
        };
        
        this.boardService.createCard(payload).subscribe({
          next: (createdCard) => {
            const cardTitle = createdCard?.title || payload.title || 'Card';
            this.toastService.success(`Card "${cardTitle}" created successfully!`);
            this.closeDialog();
            this.cardSavedEvent.emit();
          },
          error: (err) => {
            this.toastService.error('Failed to create card. Please try again.');
            this.closeDialog();
          }
        });
      }
    }
  }

  deleteCard(card: Card) {
    this.cardToDelete = card;
    this.showDeleteConfirmation = true;
  }

  confirmDeleteCard() {
    if (this.cardToDelete) {
      this.boardService.deleteCard(this.cardToDelete.id).subscribe({
        next: () => {
          this.toastService.success(`Card "${this.cardToDelete?.title}" deleted successfully!`);
          this.cancelDelete();
          this.cardDeletedEvent.emit();
        },
        error: (err) => {
          this.toastService.error('Failed to delete card. Please try again.');
          this.cancelDelete();
        }
      });
    }
  }

  cancelDelete() {
    this.showDeleteConfirmation = false;
    this.cardToDelete = null;
  }

  closeDialog() {
    this.closeDialogEvent.emit();
  }
}
