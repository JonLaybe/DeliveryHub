export interface Conversation {
  id: string;
  name: string;
  sellerPhoto?: string;
  lastMessage?: string;
  lastMessageAt: string;
  isOnline?: boolean;
  unreadMessagesCount?: number;
}