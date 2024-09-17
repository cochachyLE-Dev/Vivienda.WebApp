import { TestBed } from '@angular/core/testing';

import { PropertySaleService } from './property-sale.service';

describe('PropertySaleService', () => {
  let service: PropertySaleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PropertySaleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
