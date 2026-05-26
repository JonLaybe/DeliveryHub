export interface Conversation {
  id: string;
  name: string;
  lastMessage?: string;
  isOnline?: boolean;
  unreadMessagesCount?: number;
}