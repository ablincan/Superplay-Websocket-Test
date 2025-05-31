import { Component, inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

export enum Severity {
  Success = 'success',
  Info = 'info',
  Warn = 'warn',
  Error = 'error'
}

@Component({
    selector: 'app-toast',
    templateUrl: './toast.component.html',
    styleUrl: './toast.component.css',
    standalone: false
})
@Injectable({
  providedIn: 'root'
})
export class ToastComponent {
  private _snackBar = inject(MessageService);

  openToast(message: string, severity: Severity){
    const severityMessage = Object.keys(Severity).find(k => Severity[k as keyof typeof Severity] === severity);
    
    this._snackBar.add({
      severity: 'Notification',
      summary: severityMessage,
      detail: message,
      life: 5000
    });
  }
}
