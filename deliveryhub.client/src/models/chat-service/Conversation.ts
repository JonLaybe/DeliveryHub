export interface Conversation {
  id: string;
  name: string;
  lastMessage?: string;
  lastMessageAt: string;
  isOnline?: boolean;
  unreadMessagesCount?: number;
}