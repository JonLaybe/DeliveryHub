export interface ApplyResponseModel {
  success: boolean;
  code?: string;
  appliedAmount?: number;
  discountType?: number;
  discountUsageId?: number;
  message?: string;
}