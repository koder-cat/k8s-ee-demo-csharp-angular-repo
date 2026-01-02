import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo, CreateTodoDto, UpdateTodoDto } from '../models/todo.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private http = inject(HttpClient);
  private readonly apiUrl = '/api/todos';

  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.apiUrl);
  }

  createTodo(dto: CreateTodoDto): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, dto);
  }

  updateTodo(id: number, dto: UpdateTodoDto): Observable<Todo> {
    return this.http.patch<Todo>(`${this.apiUrl}/${id}`, dto);
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
