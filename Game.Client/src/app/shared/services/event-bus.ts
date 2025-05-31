import { Injectable } from '@angular/core';
import { Subject, Observable, BehaviorSubject } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { LoginResponsePayload } from '../models/login';
import { ResourceResponsePayload } from '../models/resource';
import { GiftResponsePayload } from '../models/gift';
import { WebSocketService } from './websockets';
import { NotificationResponsePayload } from '../models/notification';

@Injectable({ providedIn: 'root' })
export class EventBusService {
  private loginBs = new BehaviorSubject<LoginResponsePayload | null>(null);
  private resourceBs    = new BehaviorSubject<ResourceResponsePayload | null>(null);
  private giftBs   = new BehaviorSubject<GiftResponsePayload | null>(null);
  private notificationBs   = new BehaviorSubject<NotificationResponsePayload | null>(null);

  public readonly login$ = this.loginBs.asObservable().pipe(filter(v => v !== null)) as Observable<LoginResponsePayload>;
  public readonly resource$ = this.resourceBs.asObservable().pipe(filter(v => v !== null)) as Observable<ResourceResponsePayload>;
  public readonly gift$ = this.giftBs.asObservable().pipe(filter(v => v !== null)) as Observable<GiftResponsePayload>;
  public readonly notification$ = this.notificationBs.asObservable().pipe(filter(v => v !== null)) as Observable<NotificationResponsePayload>;

  constructor(private ws: WebSocketService) {
    this.ws.listenFor<LoginResponsePayload>('LoginRequest')
      .subscribe(payload => this.loginBs.next(payload));

    this.ws.listenFor<ResourceResponsePayload>('UpdateResourceRequest')
      .subscribe(payload => this.resourceBs.next(payload));

    this.ws.listenFor<GiftResponsePayload>('SendGiftRequest')
      .subscribe(payload => this.giftBs.next(payload));

    this.ws.listenFor<NotificationResponsePayload>('GiftNotification')
      .subscribe(payload => this.notificationBs.next(payload));
  }
}