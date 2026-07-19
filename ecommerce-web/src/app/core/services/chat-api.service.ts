import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ChatThread {
  id: number;
  title: string;
  createdAt: string;
  updatedAt: string;
}

export interface ChatMessage {
  id: number;
  role: string;
  content: string;
  createdAt: string;
}

export interface SendChatResponse {
  userMessage: ChatMessage;
  assistantMessage: ChatMessage;
}

@Injectable({ providedIn: 'root' })
export class ChatApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/chat`;

  listThreads(): Observable<ChatThread[]> {
    return this.http.get<ChatThread[]>(`${this.base}/threads`);
  }

  createThread(): Observable<ChatThread> {
    return this.http.post<ChatThread>(`${this.base}/threads`, {});
  }

  getMessages(threadId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.base}/threads/${threadId}/messages`);
  }

  sendMessage(threadId: number, content: string): Observable<SendChatResponse> {
    return this.http.post<SendChatResponse>(`${this.base}/threads/${threadId}/messages`, { content });
  }
}
