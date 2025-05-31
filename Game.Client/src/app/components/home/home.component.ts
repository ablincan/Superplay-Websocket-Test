import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginResponsePayload } from '../../shared/models/login';
import { EventBusService } from '../../shared/services/event-bus';
import { WebSocketService } from '../../shared/services/websockets';
import { Router } from '@angular/router';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    standalone: false
})
export class HomeComponent implements OnInit {
    loginResponse$: LoginResponsePayload = <LoginResponsePayload>{};
    resourceResponse$ = this.eventBusService.resource$;

    message: string = '';

    constructor(
        private eventBusService: EventBusService,
        private router: Router) 
    { }

    ngOnInit() {
        this.eventBusService.login$.subscribe(response => {
            this.loginResponse$ = response;
        });

        this.eventBusService.notification$.subscribe(response => {
            this.loginResponse$ = {
                ...this.loginResponse$,
                Stats: this.loginResponse$.Stats.map(stat =>
                    stat.ResourceTypeId === response.ResourceTypeId
                        ? { ...stat, Total: response.Total }
                        : stat
                )
            };
        });

        this.eventBusService.gift$.subscribe(response => {
            this.message = response.Message;
            
            if(!response.Success)
                return;
            
            this.loginResponse$ = {
                ...this.loginResponse$,
                Stats: this.loginResponse$.Stats.map(stat =>
                    stat.ResourceTypeId === response.ResourceTypeId
                        ? { ...stat, Total: response.NewTotal }
                        : stat
                )
            };
        });

        this.eventBusService.resource$.subscribe(response => {
            this.loginResponse$ = {
                ...this.loginResponse$,
                Stats: this.loginResponse$.Stats.map(stat =>
                    stat.ResourceTypeId === response.ResourceTypeId
                        ? { ...stat, Total: response.NewTotal }
                        : stat
                )
            };
        });
    }

    onTabChange() {
        this.message = '';
    }
}
