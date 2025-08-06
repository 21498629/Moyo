import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewVendorComponent } from './view-vendor.component';

describe('VendorListing', () => {
  let component: ViewVendorComponent;
  let fixture: ComponentFixture<ViewVendorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewVendorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewVendorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
