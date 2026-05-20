export interface ConversationResponse {
  conversationId: string;
  sellerId: string;
  sellerName: string;
  lastMessage: string;
  isOnline: boolean;
  unreadMessagesCount: number;
}