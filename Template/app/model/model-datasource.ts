// @ts-ignore
import { DataSource } from '@angular/cdk/collections';
// @ts-ignore
import { MatPaginator, MatSort } from '@angular/material';
// @ts-ignore
import { map } from 'rxjs/operators';
// @ts-ignore
import { Observable, of as observableOf, merge } from 'rxjs';

export interface $ModelName$Item {
  $ModelDefinition$
}

// TODO: replace this with real data from your application
const EXAMPLE_DATA: $ModelName$Item[] = [
// @ts-ignore
  $ModelDummyData$
];

/**
 * Data source for the $ModelName$ view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
export class $ModelName$DataSource extends DataSource<$ModelName$Item> {
  data: $ModelName$Item[] = EXAMPLE_DATA;

  constructor(private paginator: MatPaginator, private sort: MatSort) {
    super();
  }

  /**
   * Connect this data source to the table. The table will only update when
   * the returned stream emits new items.
   * @returns A stream of the items to be rendered.
   */
  connect(): Observable<$ModelName$Item[]> {
    // Combine everything that affects the rendered data into one update
    // stream for the data-table to consume.
    const dataMutations = [
      observableOf(this.data),
      this.paginator.page,
      this.sort.sortChange
    ];

    // Set the paginator's length
    this.paginator.length = this.data.length;

    return merge(...dataMutations).pipe(map(() => {
      return this.getPagedData(this.getSortedData([...this.data]));
    }));
  }

  /**
   *  Called when the table is being destroyed. Use this function, to clean up
   * any open connections or free any held resources that were set up during connect.
   */
  disconnect() {}

  /**
   * Paginate the data (client-side). If you're using server-side pagination,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getPagedData(data: $ModelName$Item[]) {
    const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
    return data.splice(startIndex, this.paginator.pageSize);
  }

  /**
   * Sort the data (client-side). If you're using server-side sorting,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getSortedData(data: $ModelName$Item[]) {
    if (!this.sort.active || this.sort.direction === '') {
      return data;
    }

    // @ts-ignore
    return data.sort((a, b) => {
      const isAsc = this.sort.direction === 'asc';
      // @ts-ignore
      $ModelSort$
    });
  }
}

/** Simple sort comparator for example ID/Name columns (for client-side sorting). */
function compare(a, b, isAsc) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
