// @ts-ignore
import { NgModule } from '@angular/core';
// @ts-ignore
import { Routes, RouterModule } from '@angular/router';
// @ts-ignore
import { HomeComponent } from './home/home.component';
// @ts-ignore
$ImportComponents$

const routes: Routes = [
  {path: '', component: HomeComponent },
  // @ts-ignore
  $RouteComponents$
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
// @ts-ignore
export class AppRoutingModule { }
