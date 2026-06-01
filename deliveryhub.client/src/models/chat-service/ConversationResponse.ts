export interface ConversationResponse {
  conversationId: string;
  sellerId: string;
  sellerName: string;
  lastMessage: string;
  lastMessageAt: string;
  isOnline: boolean;
  unreadMessagesCount: number;
}