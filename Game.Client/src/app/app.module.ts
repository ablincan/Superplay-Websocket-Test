import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { ToastComponent } from './shared/components/toast/toast.component';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import Aura from '@primeng/themes/aura';
import { CardModule } from 'primeng/card';
import { TabsModule } from 'primeng/tabs';
import { TabMenuModule } from 'primeng/tabmenu';
import { UpdateResourceComponent } from './components/home/update-resource/update-resource.component';
import { SendGiftComponent } from './components/home/send-gift/send-gift.component';
import { ButtonModule } from 'primeng/button';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    ToastComponent,
    UpdateResourceComponent,
    SendGiftComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule, 
    FormsModule,
    ReactiveFormsModule,
    ToastModule,
    CardModule,
    TabsModule,
    TabMenuModule,
    ButtonModule
  ],
  providers: [
    MessageService,
    provideAnimationsAsync(),
    providePrimeNG({
      theme: { 
        preset: Aura,
        options: {
          darkModeSelector: false
        }
      }
    })
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
