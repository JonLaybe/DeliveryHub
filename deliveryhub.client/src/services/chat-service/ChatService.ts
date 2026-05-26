import { CHAT_URL, CONVERSATION_URL } from "../../constants/EndpointConstants";
import { api } from "../../http";
import type { ConversationResponse } from "../../models/chat-service/ConversationResponse";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { Message } from "../../models/chat-service/Message";
import type { MessageResponse } from "../../models/chat-service/MessageResponse";

export async function getUserConversationsAsync(userId: string): Promise<Conversation[]> {
  const res = await api.get<ConversationResponse[]>(`${CHAT_URL}${CONVERSATION_URL}/${userId}`);
  const data = res.data;

  console.log("API Response (conversations):", data);

  return data.map((c) => ({
    id: c.conversationId,
    name: c.sellerName,
    lastMessage: c.lastMessage,
    isOnline: c.isOnline,
    unreadMessagesCount: c.unreadMessagesCount,
  }));
}

export async function createConversationAsync(productId: string): Promise<string> {
  const res = await api.post(`${CHAT_URL}${CONVERSATION_URL}/${productId}`);
  const data = res.data;
  
  console.log("API Response (conversations created):", data);
  
  return res.data;
}

export async function getMessagesForConversationAsync(conversationId: string, currentUserId: string): Promise<Message[]> {
  const res = await api.get<MessageResponse[]>(`${CHAT_URL}${CONVERSATION_URL}/${conversationId}/messages`);
  const data = res.data;
  
  console.log("API Response (messages):", data);
  
  return data.map((m) => ({
    id: m.messageId,
    senderName: m.senderId === currentUserId ? "Вы" : "Собеседник",
    text: m.text,
    createdAt: m.createdAt,
  }));
}