import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'love2u-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  loggedIn: boolean = false;

  constructor() { }

  ngOnInit() {
  }

}
