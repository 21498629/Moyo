import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DataService } from '../services/data-service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
  userName = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.getUserName();
  }

  getUserName() {
    this.userName = this.dataService.getCurrentUserName();
  }

  onLogout() {
    this.dataService.logout();
  }

  isAuthenticated(): boolean {
    return this.dataService.isAuthenticated();
  }

  isAdmin(): boolean {
    return this.dataService.hasRole('Administrator') || this.dataService.hasRole('Admin');
  }

  canAccessOrders(): boolean {
    return this.dataService.hasScope('api.write') || this.dataService.hasScope('api.read');
  }

  getUserRoles(): string[] {
    return this.dataService.getRoles();
  }
}