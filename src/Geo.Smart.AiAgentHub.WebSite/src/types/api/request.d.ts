export interface BaseRequest {
    [key: string]: unknown;
}

export interface ApiRequest<T = unknown> extends BaseRequest {
    data?: T;
    params?: Record<string, unknown>;
}
