export interface Pagination {
    currentPage: number;
    total: number;
    totalPage: number;
    pageSize: number;
}

export interface PageParams {
    currentPage: number;
    pageSize: number;
    keyword: string;
    sorting?: string;
    sortingDesc?: boolean;
}
