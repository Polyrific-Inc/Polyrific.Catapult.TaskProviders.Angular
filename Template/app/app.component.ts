// @ts-ignore
import { Component } from '@angular/core';
// @ts-ignore
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
// @ts-ignore
import { Observable } from 'rxjs';
// @ts-ignore
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
// @ts-ignore
export class AppComponent {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches)
    );

    title = '$Title$';

  constructor(private breakpointObserver: BreakpointObserver) {}

}