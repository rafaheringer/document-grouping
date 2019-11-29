import { TestBed } from '@angular/core/testing';

import { LearningService } from './learning.service';

describe('LearningService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LearningService = TestBed.get(LearningService);
    expect(service).toBeTruthy();
  });
});
