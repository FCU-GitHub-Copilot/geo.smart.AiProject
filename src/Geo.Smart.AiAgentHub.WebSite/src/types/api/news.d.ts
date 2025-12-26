import type { PageParams } from '@/types/common/pagination';

export interface NewsListParams extends PageParams {
    [key: string]: string | number | boolean | null | undefined;
    activeTab: string;
    isPushed: null | boolean;
}

export interface NewsCreateParams {
    description: string;
    publishEnd: string;
    publishStart: string;
    subject: string;
    summary: string;
    photoId?: string;
}

export interface NewsUpdateParams extends NewsCreateParams {
    newsId: string;
}

export interface NewsDetail extends NewsCreateParams {
    createdDate: string;
}

export interface NewsListItem extends NewsDetail {
    createdBy: string;
    isPushed: boolean;
    newsType: number;
}
