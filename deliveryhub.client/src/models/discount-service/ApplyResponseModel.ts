export interface ApplyResponseModel {
    success: boolean;
    code: string;
    appliedAmount: number;
    discountType: DiscountType;
    message: string;
}

enum DiscountType {
Percentage = 1,
FixedAmount = 2
}