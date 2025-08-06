import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DataService } from '../../services/data-service';
import { User } from '../../shared/user';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css'
})
export class RegistrationComponent {

  name = '';
  surname = '';
  email = '';
  password = '';
  address = '';
  phoneNumber = '';
  error = '';

  constructor(private service: DataService) {}

  onSubmit() {
    const payload: User = {
      name: this.name,
      surname: this.surname,
      email: this.email,
      password: this.password,
      address: this.address,
      phoneNumber: this.phoneNumber,
      createdAt: new Date()
    };
    
    this.service.register(payload).subscribe({
      next: () => {
        // Registration successful, user will be redirected by the service
      },
      error: err => {
        if (err.status === 0) {
          this.error = 'Cannot connect to server. Please check if the backend is running.';
        } else if (err.status === 400) {
          // Handle validation errors
          if (err.error?.message) {
            this.error = err.error.message;
          } else if (err.error?.errors) {
            // Handle ModelState errors
            const errors = Object.values(err.error.errors).flat();
            this.error = errors.join(', ');
          } else {
            this.error = 'Invalid registration data. Please check your inputs.';
          }
        } else if (err.status === 500) {
          // Handle Identity errors (like duplicate username)
          if (Array.isArray(err.error)) {
            const identityErrors = err.error.map((e: any) => e.description || e.code).join(', ');
            if (identityErrors.toLowerCase().includes('duplicate') || identityErrors.toLowerCase().includes('already')) {
              this.error = 'This email address is already registered. Please use a different email or try logging in.';
            } else {
              this.error = identityErrors;
            }
          } else {
            this.error = err?.error?.message ?? 'Registration failed due to server error.';
          }
        } else {
          this.error = err?.error?.message ?? 'Registration failed. Please try again.';
        }
      }
    });
  }
}
