import { Component } from '@angular/core';
import { EventBusService } from './shared/services/event-bus';
import { Severity, ToastComponent } from './shared/components/toast/toast.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    standalone: false
})
export class AppComponent {
  title = 'Game.Client';

  constructor(
    eventBusService: EventBusService,
    toastComponent: ToastComponent
  ) {
    eventBusService.notification$.subscribe(n => {
      toastComponent.openToast(n.Message, Severity.Info);
    });
  }

}
