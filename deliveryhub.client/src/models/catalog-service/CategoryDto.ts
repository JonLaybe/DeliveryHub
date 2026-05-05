import type { BaseEntityIdentityGuid } from "../BaseEntityIdentityGuid";

export interface CategoryDto extends BaseEntityIdentityGuid {
    name: string;
    parentCategoryId?: string;
}