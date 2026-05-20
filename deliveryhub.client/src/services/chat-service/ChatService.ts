import { CHAT_URL, CONVERSATION_URL } from "../../constants/EndpointConstants";
import { api } from "../../http";
import type { CreateConversationRequest } from "../../models/chat-service/CreateConversationRequest";
import type { ConversationResponse } from "../../models/chat-service/ConversationResponse";
import type { Conversation } from "../../models/chat-service/Conversation";

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