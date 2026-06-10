export interface ConversationResponse {
  conversationId: string;
  sellerId: string;
  sellerName: string;
  sellerPhoto: string;
  lastMessage: string;
  lastMessageAt: string;
  isOnline: boolean;
  unreadMessagesCount: number;
}