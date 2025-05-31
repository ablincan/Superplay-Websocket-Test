import { Component, OnInit } from '@angular/core';
import { WebSocketService } from '../../shared/services/websockets';
import { WsMessage } from '../../shared/models/ws-message';
import { LoginRequestPayload, LoginResponsePayload } from '../../shared/models/login';
import { filter, map, take } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Severity, ToastComponent } from '../../shared/components/toast/toast.component';
import { EventBusService } from '../../shared/services/event-bus';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  standalone: false
})
export class LoginComponent implements OnInit {
  deviceId: string = '';

  constructor(
    private ws: WebSocketService,
    private eventBusService: EventBusService,
    private router: Router,
    private toastComponent: ToastComponent
  ) {}

  ngOnInit() {
    this.eventBusService.login$.subscribe(response => {
        if(response.Success) {
          this.router.navigate(['/home']);
        }
        else {
          this.toastComponent.openToast(response.Error!, Severity.Error);
        }
    });
  }

  onLogin() {
    const msg: WsMessage<LoginRequestPayload> = {
      MessageType: 'LoginRequest',
      Payload: { DeviceId: this.deviceId }
    };
    this.ws.send(msg);
  }
}
