export interface Payment {
    deliveryAddress: string;
    deliveryDate: string;
    discount?: number | null;
    discountUsageId?: number | null;
}