import { Component, Input, input } from '@angular/core';
import { WebSocketService } from '../../../shared/services/websockets';
import { WsMessage } from '../../../shared/models/ws-message';
import { GiftRequestPayload } from '../../../shared/models/gift';
import { Stat } from '../../../shared/models/login';
import { Severity, ToastComponent } from '../../../shared/components/toast/toast.component';

@Component({
  selector: 'app-send-gift',
  standalone: false,
  templateUrl: './send-gift.component.html',
  styleUrl: './send-gift.component.css'
})
export class SendGiftComponent {
  @Input({ required: true }) stats: Stat[] = [];

  friendPlayerId = '';
  resourceTypeId: string | null = null;
  resourceValue: number | null = null;

  constructor(
    private ws: WebSocketService,
    private toastComponent: ToastComponent) {
  }

  sendMessage(): void {
    if(this.friendPlayerId === '' || this.resourceTypeId === null || this.resourceValue === null) {
      this.toastComponent.openToast("Please fill in all fields", Severity.Error);
      return;
    }

    const payload = {
      FriendPlayerId: this.friendPlayerId,
      ResourceTypeId: this.resourceTypeId,
      ResourceValue: this.resourceValue
    } as GiftRequestPayload;

    const msg: WsMessage<GiftRequestPayload> = {
      MessageType: 'SendGiftRequest',
      Payload: payload
    };
    this.ws.send(msg);

   this.resourceTypeId = null
   this.resourceValue = null;
  }
}
