import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from './services/todo.service';
import { Todo } from './models/todo.model';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private todoService = inject(TodoService);

  todos = signal<Todo[]>([]);
  newTodoTitle = signal('');
  loading = signal(true);
  submitting = signal(false);
  error = signal<string | null>(null);

  completedCount = computed(() => this.todos().filter(t => t.completed).length);
  totalCount = computed(() => this.todos().length);

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.loading.set(true);
    this.error.set(null);

    this.todoService.getTodos().subscribe({
      next: (todos) => {
        this.todos.set(todos);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load todos:', err);
        this.error.set('Failed to load todos. Please try again.');
        this.loading.set(false);
      }
    });
  }

  addTodo(): void {
    const title = this.newTodoTitle().trim();
    if (!title || this.submitting()) return;

    this.submitting.set(true);
    this.error.set(null);

    this.todoService.createTodo({ title }).subscribe({
      next: (todo) => {
        this.todos.update(todos => [...todos, todo]);
        this.newTodoTitle.set('');
        this.submitting.set(false);
      },
      error: (err) => {
        console.error('Failed to create todo:', err);
        this.error.set('Failed to create todo. Please try again.');
        this.submitting.set(false);
      }
    });
  }

  toggleTodo(todo: Todo): void {
    this.error.set(null);

    this.todoService.updateTodo(todo.id, { completed: !todo.completed }).subscribe({
      next: (updated) => {
        this.todos.update(todos =>
          todos.map(t => t.id === updated.id ? updated : t)
        );
      },
      error: (err) => {
        console.error('Failed to update todo:', err);
        this.error.set('Failed to update todo. Please try again.');
      }
    });
  }

  deleteTodo(id: number): void {
    this.error.set(null);

    this.todoService.deleteTodo(id).subscribe({
      next: () => {
        this.todos.update(todos => todos.filter(t => t.id !== id));
      },
      error: (err) => {
        console.error('Failed to delete todo:', err);
        this.error.set('Failed to delete todo. Please try again.');
      }
    });
  }

  dismissError(): void {
    this.error.set(null);
  }
}
