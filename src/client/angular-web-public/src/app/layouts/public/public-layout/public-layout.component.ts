import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-public-layout',
  templateUrl: './public-layout.component.html',
  styleUrls: ['./public-layout.component.css'],
  animations: [
    trigger('animation', [
      state('hidden', style({
        height: '0',
        overflow: 'hidden',
        maxHeight: '0',
        paddingTop: '0',
        paddingBottom: '0',
        marginTop: '0',
        marginBottom: '0',
        opacity: '0',
      })),
      state('void', style({
        height: '0',
        overflow: 'hidden',
        maxHeight: '0',
        paddingTop: '0',
        paddingBottom: '0',
        marginTop: '0',
        marginBottom: '0',
      })),
      state('visible', style({
        height: '*'
      })),
      transition('visible <=> hidden', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)')),
      transition('void => hidden', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)')),
      transition('void => visible', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
    ])
  ]
})
export class PublicLayoutComponent implements OnInit {

  menuActive: boolean;
  activeMenuId: string;
  routes: Array<string> = [];
  filteredRoutes: Array<string> = [];
  searchText: string;
  constructor(
    private router: Router,
  ) { }

  ngOnInit() {
    const routerRoutes = this.router.config;
    for (const route of routerRoutes) {
      if (route.children != null) {
        for (const childRoute of route.children) {
          this.routes.push(childRoute.path);
        }
      }
    }
  }

  onAnimationStart(event) {
    switch (event.toState) {
      case 'visible':
        event.element.style.display = 'block';
        break;
    }
  }
  onAnimationDone(event) {
    switch (event.toState) {
      case 'hidden':
        event.element.style.display = 'none';
        break;

      case 'void':
        event.element.style.display = 'none';
        break;
    }
  }

  toggle(id: string, dataUrl: string) {
    this.activeMenuId = (this.activeMenuId === id ? null : id);
    this.router.navigate([dataUrl]);
    this.menuActive = false;
  }

  selectRoute(routeName) {
    this.router.navigate(['/' + routeName.toLowerCase()]);
    this.filteredRoutes = [];
    this.searchText = '';
  }

  filterRoutes(event) {
    let query = event.query;
    this.filteredRoutes = this.routes.filter(route => {
      return route.toLowerCase().includes(query.toLowerCase());
    });
  }

  onMenuButtonClick(event: Event) {
    this.menuActive = !this.menuActive;
    event.preventDefault();
  }

}
