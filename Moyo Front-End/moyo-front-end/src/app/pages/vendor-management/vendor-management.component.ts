import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Vendor } from '../../shared/vendor';
import { DataService } from '../../services/data-service';

@Component({
  selector: 'app-vendor-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vendor-management.component.html',
  styleUrl: './vendor-management.component.css'
})
export class VendorManagementComponent implements OnInit {
  vendors: Vendor[] = [];
  filteredVendors: Vendor[] = [];
  searchTerm = '';
  showAddForm = false;
  editingVendor: Vendor | null = null;
  
  newVendor: Vendor = {
    id: 0,
    name: '',
    email: '',
    address: '',
    phone: ''
  };

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadVendors();
  }

  loadVendors() {
    this.dataService.GetAllVendors().subscribe({
      next: (vendors) => {
        this.vendors = vendors || [];
        this.filteredVendors = [...this.vendors];
      },
      error: (err) => {
        console.error('Error loading vendors:', err);
        this.vendors = [];
        this.filteredVendors = [];
      }
    });
  }

  onSearch() {
    if (!this.searchTerm.trim()) {
      this.filteredVendors = [...this.vendors];
    } else {
      this.filteredVendors = this.vendors.filter(vendor =>
        vendor.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        vendor.email.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
  }

  showAddVendorForm() {
    this.showAddForm = true;
    this.editingVendor = null;
    this.resetForm();
  }

  editVendor(vendor: Vendor) {
    this.editingVendor = { ...vendor };
    this.newVendor = { ...vendor };
    this.showAddForm = true;
  }

  saveVendor() {
    if (this.editingVendor) {
      this.dataService.UpdateVendor(this.newVendor).subscribe({
        next: () => {
          this.loadVendors();
          this.cancelForm();
        },
        error: (err) => console.error('Error updating vendor:', err)
      });
    } else {
      this.dataService.AddVendor(this.newVendor).subscribe({
        next: () => {
          this.loadVendors();
          this.cancelForm();
        },
        error: (err) => console.error('Error adding vendor:', err)
      });
    }
  }

  deleteVendor(id: number) {
    if (confirm('Are you sure you want to delete this vendor?')) {
      this.dataService.DeleteVendor(id).subscribe({
        next: () => this.loadVendors(),
        error: (err) => console.error('Error deleting vendor:', err)
      });
    }
  }

  cancelForm() {
    this.showAddForm = false;
    this.editingVendor = null;
    this.resetForm();
  }

  resetForm() {
    this.newVendor = {
      id: 0,
      name: '',
      email: '',
      address: '',
      phone: ''
    };
  }
}