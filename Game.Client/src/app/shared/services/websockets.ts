import { Injectable } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { filter, map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment.prod';
import { WsMessage } from '../models/ws-message';
import { EventBusService } from './event-bus';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket$: WebSocketSubject<any>;

  constructor() {
    this.socket$ = webSocket<WsMessage>(environment.target + '/game-server');
    this.connect();
  }

  private connect(): void {
    this.socket$.subscribe({
        error:    () => this.reconnect()
    });
  }

  get messages$(): Observable<WsMessage> {
    return this.socket$.asObservable();
  }

  send<T>(message: WsMessage<T>): void {
    this.socket$.next(message);
  }

  close(): void {
    this.socket$.complete();
  }

  private reconnect() {
    console.warn('[WS] reconnecting in 5s…');
    setTimeout(() => this.connect(), 5000);
  }

  public listenFor<T>(messageType: string): Observable<T> {
    return this.messages$.pipe(
      filter((m): m is WsMessage<T> => m.MessageType === messageType),
      map(m => m.Payload)
    );
  }
}
