import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgenteRegisterComponent } from './agente-register.component';

describe('AgenteRegisterComponent', () => {
  let component: AgenteRegisterComponent;
  let fixture: ComponentFixture<AgenteRegisterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AgenteRegisterComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AgenteRegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
