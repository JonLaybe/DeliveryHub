import { CHAT_URL, CONVERSATION_URL } from "../../constants/EndpointConstants";
import { api } from "../../http";
import type { CreateConversationRequest } from "../../models/chat-service/CreateConversationRequest";
import type { Conversation } from "../../models/chat-service/Conversation";

// Получить список чатов для пользователя
export async function getUserConversationsAsync(userId: string): Promise<Conversation[]> {
  const res = await api.get(`${CHAT_URL}${CONVERSATION_URL}/${userId}`);
  return res.data;
}

// Создать новый чат между покупателем и продавцом
export async function createConversationAsync(request: CreateConversationRequest): Promise<string> {
  const res = await api.post(`${CHAT_URL}${CONVERSATION_URL}`, request);
  return res.data; // возвращается conversationId
}