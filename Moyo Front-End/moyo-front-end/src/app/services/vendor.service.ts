import { Injectable } from '@angular/core';
import { Vendor } from '../shared/vendor';

@Injectable({
  providedIn: 'root'
})
export class VendorService {
  private vendors: Vendor[] = [
    {
      id: 1,
      name: 'ABC Suppliers',
      email: 'contact@abcsuppliers.com',
      address: '123 Business Street, City, Country',
      phone: '+1234567890'
    },
    {
      id: 2,
      name: 'XYZ Trading',
      email: 'info@xyztrading.com',
      address: '456 Commerce Ave, City, Country',
      phone: '+0987654321'
    }
  ];

  getAllVendors(): Vendor[] {
    return [...this.vendors];
  }

  getVendorById(id: number): Vendor | undefined {
    return this.vendors.find(v => v.id === id);
  }

  addVendor(vendor: Vendor): void {
    this.vendors.push({ ...vendor });
  }

  updateVendor(vendor: Vendor): void {
    const index = this.vendors.findIndex(v => v.id === vendor.id);
    if (index !== -1) {
      this.vendors[index] = { ...vendor };
    }
  }

  deleteVendor(id: number): void {
    this.vendors = this.vendors.filter(v => v.id !== id);
  }

  searchVendors(searchTerm: string): Vendor[] {
    return this.vendors.filter(v => 
      v.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      v.email.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }
}