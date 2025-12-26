/**
 * 頁面權限管理皆透過角色Key判斷
 */
export const roleType = {
    user: '0ffbf804-49dd-41a7-a388-21a28c908002',
    agencyManager: '00fbf804-49dd-41a7-a388-21a28c908001',
    systemManager: '000bf804-49dd-41a7-a388-21a28c908000',
} as const;

export type RoleTypeKey = keyof typeof roleType;

export interface Role {
    id: string;
    name: string;
    key: RoleTypeKey;
    permissions: string[];
    description?: string;
}
