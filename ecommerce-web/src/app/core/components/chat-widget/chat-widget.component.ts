import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../shared/material.module';
import { AdminAuthService } from '../../services/admin-auth.service';
import { CustomerAuthService } from '../../services/customer-auth.service';
import { ChatApiService, ChatMessage, ChatThread } from '../../services/chat-api.service';

@Component({
  selector: 'ecom-chat-widget',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule],
  templateUrl: './chat-widget.component.html',
  styleUrl: './chat-widget.component.scss'
})
export class ChatWidgetComponent implements OnInit {
  private readonly chatApi = inject(ChatApiService);
  private readonly customerAuth = inject(CustomerAuthService);
  private readonly adminAuth = inject(AdminAuthService);

  open = false;
  threads: ChatThread[] = [];
  messages: ChatMessage[] = [];
  activeThreadId: number | null = null;
  draft = '';
  busy = false;
  error = '';

  get visible(): boolean {
    return this.customerAuth.isLoggedIn() || this.adminAuth.isAdminSession();
  }

  ngOnInit(): void {
    // lazy-load threads when opened
  }

  toggle(): void {
    this.open = !this.open;
    if (this.open && this.threads.length === 0) {
      this.refreshThreads();
    }
  }

  refreshThreads(): void {
    this.chatApi.listThreads().subscribe({
      next: (t) => (this.threads = t),
      error: () => (this.error = 'Could not load chats.')
    });
  }

  newChat(): void {
    this.busy = true;
    this.error = '';
    this.chatApi.createThread().subscribe({
      next: (t) => {
        this.busy = false;
        this.threads = [t, ...this.threads];
        this.selectThread(t.id);
      },
      error: () => {
        this.busy = false;
        this.error = 'Could not create chat.';
      }
    });
  }

  selectThread(id: number): void {
    this.activeThreadId = id;
    this.messages = [];
    this.chatApi.getMessages(id).subscribe({
      next: (m) => (this.messages = m),
      error: () => (this.error = 'Could not load messages.')
    });
  }

  send(): void {
    const text = this.draft.trim();
    if (!text || this.busy) return;

    const ensureThread$ =
      this.activeThreadId == null
        ? this.chatApi.createThread()
        : null;

    const continueSend = (threadId: number) => {
      this.busy = true;
      this.draft = '';
      this.chatApi.sendMessage(threadId, text).subscribe({
        next: (res) => {
          this.busy = false;
          this.messages = [...this.messages, res.userMessage, res.assistantMessage];
          this.refreshThreads();
        },
        error: (e) => {
          this.busy = false;
          this.error = e.error?.error ?? e.message ?? 'Chat request failed.';
        }
      });
    };

    if (ensureThread$) {
      this.busy = true;
      ensureThread$.subscribe({
        next: (t) => {
          this.threads = [t, ...this.threads];
          this.activeThreadId = t.id;
          continueSend(t.id);
        },
        error: () => {
          this.busy = false;
          this.error = 'Could not start chat.';
        }
      });
    } else {
      continueSend(this.activeThreadId!);
    }
  }
}
