import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { HeaderComponent } from './presentation/components/header/header.component';
import * as AuthActions from './store/auth/auth.actions';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('ShoppingCartUI');

  constructor(private store: Store) {}

  ngOnInit(): void {
    // Restaurar el estado de autenticación desde localStorage al arrancar
    console.log('App: Inicializando autenticación desde localStorage');
    this.store.dispatch(AuthActions.restoreAuthFromStorage());
  }
}
