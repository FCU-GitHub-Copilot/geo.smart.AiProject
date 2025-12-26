export interface News {
    id: string;
    subject: string;
    summary: string;
    description: string;
    publishStart: string;
    publishEnd: string;
    photoId?: string;
    createdDate: string;
    createdBy: string;
    isPushed: boolean;
    newsType: number;
}
