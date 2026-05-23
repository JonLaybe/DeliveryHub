import type { Conversation } from "./Conversation";

export interface ChatListProps {
  userId: string;
  onSelectConversation: (conversation: Conversation) => void;
}