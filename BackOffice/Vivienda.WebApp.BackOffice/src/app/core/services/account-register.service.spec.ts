import { TestBed } from '@angular/core/testing';

import { AccountRegisterService } from './account-register.service';

describe('AccountRegisterService', () => {
  let service: AccountRegisterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AccountRegisterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
