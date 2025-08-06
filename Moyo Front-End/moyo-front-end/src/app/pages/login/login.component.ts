import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DataService } from '../../services/data-service';
import { Login } from '../../shared/login';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(private service: DataService) {}

  onSubmit() {
    const payload: Login = { userName: this.email, password: this.password };
    this.service.login(payload).subscribe({
      error: err => {
        if (err.status === 0) {
          this.error = 'Cannot connect to server. Please check if the backend is running.';
        } else {
          this.error = err?.error?.message ?? 'Login failed';
        }
      }
    });
  }
}
