import { CHAT_URL, CONVERSATION_URL } from "../../constants/EndpointConstants";
import { api } from "../../http";
import type { CreateConversationRequest } from "../../models/chat-service/CreateConversationRequest";
import type { ConversationResponse } from "../../models/chat-service/ConversationResponse";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { Message } from "../../models/chat-service/Message";
import type { MessageResponse } from "../../models/chat-service/MessageResponse";

export async function getUserConversationsAsync(userId: string): Promise<Conversation[]> {
  const res = await api.get<ConversationResponse[]>(`${CHAT_URL}${CONVERSATION_URL}/${userId}`);
  const data = res.data;

  console.log("API Response:", data);

  return data.map((c) => ({
    id: c.conversationId,
    name: c.sellerName, 
    lastMessage: c.lastMessage,
  }));
}

export async function createConversationAsync(request: CreateConversationRequest): Promise<string> {
  const res = await api.post(`${CHAT_URL}${CONVERSATION_URL}`, request);
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