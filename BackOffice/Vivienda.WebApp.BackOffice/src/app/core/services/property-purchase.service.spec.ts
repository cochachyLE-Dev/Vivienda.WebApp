import { TestBed } from '@angular/core/testing';

import { PropertyPurchaseService } from './property-purchase.service';

describe('PropertyPurchaseService', () => {
  let service: PropertyPurchaseService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PropertyPurchaseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
