import type { Conversation } from "../../models/chat-service/Conversation";

export interface ChatWindowProps {
  conversation: Conversation;
  currentUserId: string;
}