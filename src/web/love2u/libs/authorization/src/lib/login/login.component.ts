import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Component({
  template: `
    <p>{{ errorMessage | async }}</p>
  `,
  styles: [],
  encapsulation: ViewEncapsulation.None
})
export class LoginComponent implements OnInit {
  public errorMessage = new BehaviorSubject<string>(null);

  constructor() { }

  ngOnInit() {
  }

}
