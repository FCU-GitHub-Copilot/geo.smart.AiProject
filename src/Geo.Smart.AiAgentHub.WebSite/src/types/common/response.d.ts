export interface BaseResponse<T = unknown> {
    success: boolean;
    message: string;
    data?: T;
}

export interface PaginatedResponse<T = unknown> extends BaseResponse<T[]> {
    pagination?: {
        currentPage: number;
        total: number;
        totalPage: number;
        pageSize: number;
    };
}

export interface ApiResponse<T = unknown> extends BaseResponse<T> {
    code?: string | number;
    timestamp?: string;
}
