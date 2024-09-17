import { TestBed } from '@angular/core/testing';

import { PropertySaleOfferService } from './property-sale-offer.service';

describe('PropertySaleOfferService', () => {
  let service: PropertySaleOfferService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PropertySaleOfferService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
