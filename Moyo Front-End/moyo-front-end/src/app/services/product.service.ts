import { Injectable } from '@angular/core';
import { Product } from '../shared/product';
import { ProductCategory } from '../shared/product-category';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private products: Product[] = [
    {
      id: 1,
      name: 'Sample Product 1',
      description: 'This is a sample product',
      price: '99.99',
      image: '',
      quantity: 10,
      isActive: true,
      categoryId: 1,
      venderId: 1
    },
    {
      id: 2,
      name: 'Sample Product 2',
      description: 'Another sample product',
      price: '149.99',
      image: '',
      quantity: 5,
      isActive: true,
      categoryId: 2,
      venderId: 2
    }
  ];

  getAllProducts(): Product[] {
    return [...this.products];
  }

  getProductById(id: number): Product | undefined {
    return this.products.find(p => p.id === id);
  }

  addProduct(product: Product): void {
    this.products.push({ ...product });
  }

  updateProduct(product: Product): void {
    const index = this.products.findIndex(p => p.id === product.id);
    if (index !== -1) {
      this.products[index] = { ...product };
    }
  }

  deleteProduct(id: number): void {
    this.products = this.products.filter(p => p.id !== id);
  }

  getProductsByCategory(categoryId: number): Product[] {
    return this.products.filter(p => p.categoryId === categoryId);
  }

  getProductsByVendor(vendorId: number): Product[] {
    return this.products.filter(p => p.venderId === vendorId);
  }

  searchProducts(searchTerm: string): Product[] {
    return this.products.filter(p => 
      p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.description.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }
}