import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MyProfileModel } from 'src/app/models/my-profile-model';
import { UserService } from 'src/app/modules/user/user.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MessageService } from 'src/app/primeng/components/common/api';
import { AuthenticationService } from 'src/app/modules/authentication/authentication.service';
import { SignOutOption } from 'src/app/value-objects/sign-out-option.enum';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.css'],
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
export class AdminLayoutComponent implements OnInit {

  counter: number; 

  profileModel = new MyProfileModel();
  menuActive: boolean;
  activeMenuId: string;
  routes: Array<string> = [];
  filteredRoutes: Array<string> = [];
  searchText: string;
  loading = true;
  constructor(
    private router: Router,
    private userService: UserService,
    private globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private messageService: MessageService,
    private authenticationService: AuthenticationService,
  ) {
  }

  ngOnInit() {
    this.resetCounter();
    setInterval(() => {
      this.counter--;
      if (this.counter === 0) {
        this.authenticationService.signOut(SignOutOption.TimeOut).subscribe();
        this.messageService.clear();
        this.messageService.add({
          severity: 'success',
          summary: this.globalizationDictionaryPipe.transform('Success'),
          detail: this.globalizationMessagesPipe.transform('InfoLogoutOperationSuccessful')
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle')
        });
        setTimeout(() => {
          this.router.navigate(['/Home']);
        }, 1000);
      }
    }, 1000);


    const routerRoutes = this.router.config;
    for (const route of routerRoutes) {
      if (route.children != null) {
        for (const childRoute of route.children) {
          this.routes.push(childRoute.path);
        }
      }
    }

    if (sessionStorage.getItem('myProfile')) {
      const jsonObj: MyProfileModel = JSON.parse(
        sessionStorage.getItem('myProfile')
      );
      this.profileModel = jsonObj as MyProfileModel;
      this.loading = false;
    } else {
      this.userService.myProfile().subscribe(
        res => {
          this.loading = false;
          if (res.status === 200) {
            this.profileModel = res.body as MyProfileModel;
            sessionStorage.setItem('myProfile', JSON.stringify(this.profileModel));
          } else {
            this.messageService.add({
              severity: 'error',
              summary: this.globalizationDictionaryPipe.transform('Error'),
              detail: 'IN01 ' + res.statusText
            });
          }
        },
        err => {
          this.loading = false;
          if (err.status === 400) {
            if (err.error != null) {
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN02 ' + err.error.message
              });
            } else {
              this.messageService.add({
                severity: 'error',
                summary: this.globalizationDictionaryPipe.transform('Error'),
                detail: 'IN03 ' + err
              });
            }
          }
        }
      );
    }
  }

  showConfirm() {
    this.messageService.clear();
    this.messageService.add(
      {
        key: 'c',
        sticky: true,
        severity: 'warn',
        summary: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
        detail: this.globalizationMessagesPipe.transform('QuestionAreYouSureLogout')
      });
  }

  resetCounter() {
    this.counter = 1200;
  }

  onConfirm() {
    this.messageService.clear('c');
    this.signOut();
  }

  onReject() {
    this.messageService.clear('c');
  }

  clear() {
    this.messageService.clear();
  }

  signOut() {
    this.authenticationService.signOut(SignOutOption.ValidLogout).subscribe();
    this.messageService.clear();
    this.messageService.add({
      severity: 'success',
      summary: this.globalizationDictionaryPipe.transform('Success'),
      detail: this.globalizationMessagesPipe.transform('InfoLogoutOperationSuccessful')
        + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle')
    });
    setTimeout(() => {
      this.router.navigate(['/Home']);
    }, 1000);
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

  toggle(id: string) {
    this.activeMenuId = (this.activeMenuId === id ? null : id);
  }

  onKeydown(event: KeyboardEvent, id: string) {
    if (event.which === 32 || event.which === 13) {
      this.toggle(id);
      event.preventDefault();
    }
  }

  selectRoute(routeName) {
    this.router.navigate(['/' + routeName.toLowerCase()]);
    this.filteredRoutes = [];
    this.searchText = "";
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
