export interface ConversationResponse {
  id: string;
  sellerId: string;
  sellerName: string;
  lastMessage: string;
  isOnline: boolean;
  unreadMessagesCount: number;
}