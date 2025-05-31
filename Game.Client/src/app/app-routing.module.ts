import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';

const routes: Routes = [
  {
      title: 'Login',
      path: '',
      component: LoginComponent,
      canActivate: []
    },
    {
      title: 'Home',
      path: 'home',
      component: HomeComponent,
      canActivate: []
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
