import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../shared/product';
import { ProductCategory } from '../../shared/product-category';
import { Vendor } from '../../shared/vendor';
import { DataService } from '../../services/data-service';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-management.component.html',
  styleUrl: './product-management.component.css'
})
export class ProductManagementComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  categories: ProductCategory[] = [];
  vendors: Vendor[] = [];
  searchTerm = '';
  showAddForm = false;
  editingProduct: Product | null = null;
  
  newProduct: Product = {
    id: 0,
    name: '',
    description: '',
    price: '',
    image: '',
    quantity: 0,
    isActive: true,
    categoryId: 0,
    venderId: 0
  };

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.loadProducts();
    this.loadCategories();
    this.loadVendors();
  }

  loadProducts() {
    this.dataService.GetAllProducts().subscribe({
      next: (products) => {
        this.products = products || [];
        this.filteredProducts = [...this.products];
      },
      error: (err) => {
        console.error('Error loading products:', err);
        this.products = [];
        this.filteredProducts = [];
      }
    });
  }

  loadCategories() {
    this.dataService.GetAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories || [];
      },
      error: (err) => {
        console.error('Error loading categories:', err);
        this.categories = [];
      }
    });
  }

  loadVendors() {
    this.dataService.GetAllVendors().subscribe({
      next: (vendors) => {
        this.vendors = vendors || [];
      },
      error: (err) => {
        console.error('Error loading vendors:', err);
        this.vendors = [];
      }
    });
  }

  onSearch() {
    if (!this.searchTerm.trim()) {
      this.filteredProducts = [...this.products];
    } else {
      this.filteredProducts = this.products.filter(product =>
        product.name.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
  }

  showAddProductForm() {
    this.showAddForm = true;
    this.editingProduct = null;
    this.resetForm();
  }

  editProduct(product: Product) {
    this.editingProduct = { ...product };
    this.newProduct = { ...product };
    this.showAddForm = true;
  }

  saveProduct() {
    if (this.editingProduct) {
      this.dataService.UpdateProduct(this.newProduct).subscribe({
        next: () => {
          this.loadProducts();
          this.cancelForm();
        },
        error: (err) => console.error('Error updating product:', err)
      });
    } else {
      this.dataService.AddProduct(this.newProduct).subscribe({
        next: () => {
          this.loadProducts();
          this.cancelForm();
        },
        error: (err) => console.error('Error adding product:', err)
      });
    }
  }

  deleteProduct(id: number) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.dataService.DeleteProduct(id).subscribe({
        next: () => this.loadProducts(),
        error: (err) => console.error('Error deleting product:', err)
      });
    }
  }

  cancelForm() {
    this.showAddForm = false;
    this.editingProduct = null;
    this.resetForm();
  }

  resetForm() {
    this.newProduct = {
      id: 0,
      name: '',
      description: '',
      price: '',
      image: '',
      quantity: 0,
      isActive: true,
      categoryId: 0,
      venderId: 0
    };
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.name : 'Unknown';
  }

  getVendorName(vendorId: number): string {
    const vendor = this.vendors.find(v => v.id === vendorId);
    return vendor ? vendor.name : 'Unknown';
  }
}