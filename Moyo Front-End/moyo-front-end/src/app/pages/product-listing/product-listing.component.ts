import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../services/data-service';
import { Product } from '../../shared/product';
import { Vendor } from '../../shared/vendor';
import { ProductCategory } from '../../shared/product-category';

@Component({
  selector: 'app-product-listing',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-listing.component.html',
  styleUrl: './product-listing.component.css'
})
export class ProductListing implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  vendors: Vendor[] = [];
  categories: ProductCategory[] = [];
  productQuantities: { [key: number]: number } = {};
  console = console;
  
  searchTerm = '';
  selectedVendor = '';
  selectedCategory = '';
  
  constructor(private dataService: DataService) {}
  
  ngOnInit() {
    this.loadData();
  }
  
  loadData() {
    console.log('Loading products...');
    this.dataService.GetAllProducts().subscribe({
      next: (products) => {
        console.log('Products loaded:', products);
        if (products && products.length > 0) {
          console.log('Sample product:', products[0]);
        }
        this.products = products || [];
        this.filteredProducts = products || [];
      },
      error: (err) => {
        console.error('Error loading products:', err);
        this.products = [];
        this.filteredProducts = [];
      }
    });
    
    this.dataService.GetAllVendors().subscribe({
      next: (vendors) => {
        console.log('Vendors loaded:', vendors);
        if (vendors && vendors.length > 0) {
          console.log('Sample vendor:', vendors[0]);
        }
        this.vendors = vendors || [];
      },
      error: (err) => {
        console.error('Error loading vendors:', err);
        this.vendors = [];
      }
    });
    
    this.dataService.GetAllCategories().subscribe({
      next: (categories) => {
        console.log('Categories loaded:', categories);
        if (categories && categories.length > 0) {
          console.log('Sample category:', categories[0]);
        }
        this.categories = categories || [];
      },
      error: (err) => {
        console.error('Error loading categories:', err);
        this.categories = [];
      }
    });
  }
  
  applyFilters() {
    console.log('Applying filters:', {
      searchTerm: this.searchTerm,
      selectedVendor: this.selectedVendor,
      selectedCategory: this.selectedCategory
    });
    
    let filteredProducts = [...this.products];
    
    // Apply search filter
    if (this.searchTerm) {
      filteredProducts = filteredProducts.filter(product => 
        product.name.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
      console.log('After search filter:', filteredProducts.length);
    }
    
    // Apply vendor filter
    if (this.selectedVendor) {
      filteredProducts = filteredProducts.filter(product => {
        const vendorId = (product as any).venderId || (product as any).vendorId;
        console.log('Comparing vendor ID:', vendorId, 'with selected:', this.selectedVendor);
        return vendorId?.toString() === this.selectedVendor;
      });
      console.log('After vendor filter:', filteredProducts.length);
    }
    
    // Apply category filter
    if (this.selectedCategory) {
      filteredProducts = filteredProducts.filter(product => {
        const categoryId = (product as any).categoryId || (product as any).productCategoryId;
        console.log('Comparing category ID:', categoryId, 'with selected:', this.selectedCategory);
        return categoryId?.toString() === this.selectedCategory;
      });
      console.log('After category filter:', filteredProducts.length);
    }
    
    this.filteredProducts = filteredProducts;
    console.log('Final filtered products:', this.filteredProducts.length);
  }
  
  onSearchChange() {
    this.applyFilters();
  }
  
  onVendorChange() {
    this.applyFilters();
  }
  
  onCategoryChange() {
    this.applyFilters();
  }
  
  clearFilters() {
    this.searchTerm = '';
    this.selectedVendor = '';
    this.selectedCategory = '';
    this.filteredProducts = this.products;
  }
  
  getProductQuantity(productId: number): number {
    return this.productQuantities[productId] || 1;
  }
  
  increaseQuantity(productId: number) {
    this.productQuantities[productId] = (this.productQuantities[productId] || 1) + 1;
  }
  
  decreaseQuantity(productId: number) {
    const current = this.productQuantities[productId] || 1;
    if (current > 1) {
      this.productQuantities[productId] = current - 1;
    }
  }
  
  addToOrder(product: Product) {
    const quantity = this.getProductQuantity(product.id);
    console.log(`Adding ${quantity} of ${product.name} to order`);
    // This button doesn't work as requested
  }
  
  getImageUrl(image: string): string {
    if (!image) {
      console.log('No image provided');
      return '';
    }
    const url = `https://localhost:7115/${image}`;
    console.log('Generated image URL:', url, 'for file:', image);
    return url;
  }
  
  onImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.style.display = 'none';
    }
  }
}
