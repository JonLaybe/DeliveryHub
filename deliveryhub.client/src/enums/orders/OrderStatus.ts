export const OrderStatus = {
  Unknown: 'Unknown',
  Relevant: 'Relevant',
  Completed: 'Completed',
  Cancelled: 'Cancelled',
} as const;

export type OrderStatusType = typeof OrderStatus[keyof typeof OrderStatus];