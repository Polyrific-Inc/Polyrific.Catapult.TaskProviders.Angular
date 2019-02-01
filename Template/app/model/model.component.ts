// @ts-ignore
import { Component, OnInit, ViewChild } from '@angular/core';
// @ts-ignore
import { MatPaginator, MatSort } from '@angular/material';
// @ts-ignore
import { $ModelClassName$DataSource } from './$ModelName$-datasource';

@Component({
  selector: 'app-$ModelName$',
  templateUrl: './$ModelName$.component.html',
  styleUrls: ['./$ModelName$.component.css']
})
// @ts-ignore
export class $ModelClassName$Component implements OnInit {
  // @ts-ignore
  @ViewChild(MatPaginator) paginator: MatPaginator;
  // @ts-ignore
  @ViewChild(MatSort) sort: MatSort;
  dataSource: $ModelClassName$DataSource;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  // @ts-ignore
  displayedColumns = [$PropertyList$];

  ngOnInit() {
    this.dataSource = new $ModelClassName$DataSource(this.paginator, this.sort);
  }
}
