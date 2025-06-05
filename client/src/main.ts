import { Component } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { BoardComponent } from './app/features/board/board.component';
import { provideHttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [BoardComponent],
  template: `
    <app-board></app-board>
  `
})
export class App {}

bootstrapApplication(App, {
  providers: [provideHttpClient()]
});