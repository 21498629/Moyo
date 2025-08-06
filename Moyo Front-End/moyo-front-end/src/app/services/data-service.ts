import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, map, Observable, Subject, tap, catchError, throwError } from 'rxjs';
import { User } from '../shared/user';
import { Login } from '../shared/login';
import { ProductCategory } from '../shared/product-category';
import { Vendor } from '../shared/vendor';
import { Product } from '../shared/product';
import { Order } from '../shared/order';
import { OrderItem } from '../shared/order-item';

const TOKEN_KEY = 'auth-token';
const ROLES_KEY = 'auth-roles';
const SCOPES_KEY = 'auth-scopes';

export interface LoginResponse { token: string; roles?: string[]; scopes?: string[]; }
export interface RegisterResponse { message: string; token: string; }

@Injectable({
    providedIn: 'root'
})

export class DataService {

    apiUrl = 'https://localhost:7115/api/';

    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        })
    };

    constructor(private httpClient: HttpClient, private router: Router, @Inject(PLATFORM_ID) private platformId: Object) { }

    // LOG IN
    login(payload: Login) {
        return this.httpClient.post<LoginResponse>(`${this.apiUrl}Users/Login`, payload).pipe(
        tap(res => {
            this.setToken(res.token);
            this.setRoles(res.roles ?? []);
            this.setScopes(res.scopes ?? []);
            this.router.navigateByUrl('/landing');
        }),
        catchError(err => throwError(() => err))
        );
    }

    // REGISTER
    register(payload: User) {
        return this.httpClient.post<RegisterResponse>(`${this.apiUrl}Users/Register`, payload).pipe(
        tap(res => {
            if (res?.token) {
            this.setToken(res.token);
            this.setRoles([]);
            this.setScopes(['api.read']);
            }
            this.router.navigateByUrl('/landing');
        }),
        catchError(err => throwError(() => err))
        );
    }

    logout() {
        if (isPlatformBrowser(this.platformId)) {
            localStorage.removeItem(TOKEN_KEY);
            localStorage.removeItem(ROLES_KEY);
            localStorage.removeItem(SCOPES_KEY);
        }
        this.router.navigateByUrl('/login');
    }

    get token(): string | null { 
        return isPlatformBrowser(this.platformId) ? localStorage.getItem(TOKEN_KEY) : null;
    }
    
    getCurrentUserName(): string {
        const token = this.token;
        if (!token) return '';
        try {
            const payload = this.decodeJwt(token);
            console.log('JWT payload:', payload);
            return payload.name || payload.sub || 'User';
        } catch {
            return 'User';
        }
    }

    getRoles(): string[] {
        if (!isPlatformBrowser(this.platformId)) return [];
        const roles = localStorage.getItem(ROLES_KEY);
        return roles ? JSON.parse(roles) : [];
    }

    getScopes(): string[] {
        if (!isPlatformBrowser(this.platformId)) return [];
        const scopes = localStorage.getItem(SCOPES_KEY);
        return scopes ? JSON.parse(scopes) : [];
    }

    hasRole(role: string): boolean {
        return this.getRoles().includes(role);
    }

    hasScope(scope: string): boolean {
        return this.getScopes().includes(scope);
    }
    
    isAuthenticated(): boolean {
        const t = this.token; if (!t) return false;
        try {
        const p = this.decodeJwt(t);
        const now = Math.floor(Date.now() / 1000);
        return !p.exp || p.exp > now;
        } catch { return false; }
    }
    private setToken(v: string){ 
        if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(TOKEN_KEY, v);
        }
    }
    private setRoles(v: string[]){ 
        if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(ROLES_KEY, JSON.stringify(v));
        }
    }
    private setScopes(v: string[]){ 
        if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(SCOPES_KEY, JSON.stringify(v));
        }
    }
    

    


    private decodeJwt(token: string): any {
        const [, payload] = token.split('.');
        const normalized = payload.replace(/-/g, '+').replace(/_/g, '/')
        .padEnd(Math.ceil(payload.length / 4) * 4, '=');
        return JSON.parse(atob(normalized));
    }

    // LOG OUT

    // PRODUCT CATEGORY
    GetAllCategories(): Observable<ProductCategory[]> {
        return this.httpClient.get<ProductCategory[]>(`${this.apiUrl}ProductCategories/GetAllProductCategories`);
    }

    // VENDOR
    GetAllVendors(): Observable<Vendor[]> {
        return this.httpClient.get<Vendor[]>(`${this.apiUrl}Vendors/GetAllVendors`);
    }

    // PRODUCT
    GetAllProducts(): Observable<Product[]> {
        return this.httpClient.get<Product[]>(`${this.apiUrl}Products/GetAllProducts`);
    }

    // Synchronous methods for local data
    getCategories(): ProductCategory[] {
        return [
            { id: 1, name: 'Electronics', description: 'Electronic devices and accessories' },
            { id: 2, name: 'Clothing', description: 'Apparel and fashion items' },
            { id: 3, name: 'Home & Garden', description: 'Home improvement and garden supplies' },
            { id: 4, name: 'Sports', description: 'Sports equipment and accessories' }
        ];
    }

    getVendors(): Vendor[] {
        return [
            { id: 1, name: 'ABC Suppliers', email: 'contact@abcsuppliers.com', address: '123 Business Street', phone: '+1234567890' },
            { id: 2, name: 'XYZ Trading', email: 'info@xyztrading.com', address: '456 Commerce Ave', phone: '+0987654321' },
            { id: 3, name: 'Global Imports', email: 'sales@globalimports.com', address: '789 Trade Center', phone: '+1122334455' }
        ];
    }

    // VENDOR CRUD OPERATIONS
    AddVendor(vendor: Vendor): Observable<Vendor> {
        return this.httpClient.post<Vendor>(`${this.apiUrl}Vendors/AddVendor`, vendor, this.httpOptions);
    }

    UpdateVendor(vendor: Vendor): Observable<Vendor> {
        return this.httpClient.put<Vendor>(`${this.apiUrl}Vendors/UpdateVendor`, vendor, this.httpOptions);
    }

    DeleteVendor(id: number): Observable<any> {
        return this.httpClient.delete(`${this.apiUrl}Vendors/DeleteVendor/${id}`, this.httpOptions);
    }

    // PRODUCT CRUD OPERATIONS
    AddProduct(product: Product): Observable<Product> {
        return this.httpClient.post<Product>(`${this.apiUrl}Products/AddProduct`, product, this.httpOptions);
    }

    UpdateProduct(product: Product): Observable<Product> {
        return this.httpClient.put<Product>(`${this.apiUrl}Products/UpdateProduct`, product, this.httpOptions);
    }

    DeleteProduct(id: number): Observable<any> {
        return this.httpClient.delete(`${this.apiUrl}Products/DeleteProduct/${id}`, this.httpOptions);
    }

    // ORDER

    // ORDER ITEMS
}