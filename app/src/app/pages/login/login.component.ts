import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FormsModule } from "@angular/forms";
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class Login {

  loginObj: LoginModel = new LoginModel();
  loginStatusMessage = signal<string | null>(null);

  // 1. Injete o HttpClient aqui
  constructor(private router: Router, private http: HttpClient) { }

  onLogin() {
    this.loginStatusMessage.set(null); // Limpa a mensagem anterior

    // 2. Chame o endpoint da sua API com http.post
    this.http.post('http://localhost:5033/api/v1/User/login', this.loginObj).subscribe({
      
      // 3. Callback de SUCESSO (login deu certo)
      next: (res: any) => {
        // O backend provavelmente retornará dados do usuário ou um token JWT
        // Aqui, guardamos a resposta no localStorage para manter o usuário logado
        localStorage.setItem('loggedUser', JSON.stringify(res));

        this.loginStatusMessage.set("Login bem-sucedido! Redirecionando...");
        this.router.navigateByUrl('/home');
      },

      // 4. Callback de ERRO (login falhou)
      error: (err) => {
        console.error("Erro no login:", err); // Loga o erro para depuração
        this.loginStatusMessage.set("Email ou senha inválidos. Tente novamente.");
      }

    });
  }
}

export class LoginModel {
  email: string;
  password: string;

  constructor() {
    this.email = "";
    this.password = ""
  }
}