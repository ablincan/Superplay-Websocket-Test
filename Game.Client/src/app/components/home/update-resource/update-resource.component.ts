import { Component, Input } from '@angular/core';
import { WsMessage } from '../../../shared/models/ws-message';
import { ResourceRequestPayload } from '../../../shared/models/resource';
import { WebSocketService } from '../../../shared/services/websockets';
import { Stat } from '../../../shared/models/login';
import { Toast } from 'primeng/toast';
import { Severity, ToastComponent } from '../../../shared/components/toast/toast.component';

@Component({
  selector: 'app-update-resource',
  standalone: false,
  templateUrl: './update-resource.component.html',
  styleUrl: './update-resource.component.css'
})
export class UpdateResourceComponent {
  @Input({ required: true }) stats: Stat[] = [];
  
  resourceTypeId: string | null = null;
  delta: number | null = null;

  constructor(
    private ws: WebSocketService,
    private toastComponent: ToastComponent) 
  { }

  sendMessage(): void {
    if(this.resourceTypeId === '' || this.delta === null) {
      this.toastComponent.openToast("Please fill in all fields", Severity.Error);
      return;
    }

    const payload = {
      ResourceTypeId: this.resourceTypeId,
      Delta: this.delta
    } as ResourceRequestPayload;
    
    const msg: WsMessage<ResourceRequestPayload> = {
      MessageType: 'UpdateResourceRequest',
      Payload: payload
    };
    this.ws.send(msg);

    this.resourceTypeId = null
    this.delta = null;
  }
}
