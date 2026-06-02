import { CHAT_URL, CONVERSATION_URL } from "../../constants/EndpointConstants";
import { api_authorized  } from "../../http";
import type { ConversationResponse } from "../../models/chat-service/ConversationResponse";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { Message } from "../../models/chat-service/Message";
import type { MessageResponse } from "../../models/chat-service/MessageResponse";
import { getAccessToken } from "../../services/auth-service/AuthService";

export async function getUserConversationsAsync(): Promise<Conversation[]> {
  const res = await api_authorized.get<ConversationResponse[]>(`${CHAT_URL}${CONVERSATION_URL}`);
  const data = res.data;

  console.log("API Response (conversations):", data);

  return data.map((c) => ({
    id: c.conversationId,
    name: c.sellerName,
    lastMessage: c.lastMessage,
	lastMessageAt: c.lastMessageAt,
    isOnline: c.isOnline,
    unreadMessagesCount: c.unreadMessagesCount,
  }));
}

export async function createConversationAsync(productId: string): Promise<string> {
  const res = await api_authorized.post(`${CHAT_URL}${CONVERSATION_URL}/${productId}`);
  const data = res.data;
  
  console.log("API Response (conversations created):", data);
  
  return res.data;
}

export async function getMessagesForConversationAsync(conversationId: string): Promise<Message[]> {
  const res = await api_authorized.get<MessageResponse[]>(`${CHAT_URL}${CONVERSATION_URL}/${conversationId}/messages`);
  const data = res.data;
  
  console.log("API Response (messages):", data);
  
  const currentUserId = getCurrentUserIdFromToken();
  
  return data.map((m) => ({
    id: m.messageId,
    senderName: m.senderId === currentUserId ? "Вы" : "Собеседник",
    text: m.text,
    createdAt: m.createdAt,
  }));
}

function getCurrentUserIdFromToken(): string | null {
  const token = getAccessToken();
  if (!token) return null;
  
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.uid || payload.userId || payload.nameid || null;
  } catch (error) {
    console.error("Failed to parse token:", error);
    return null;
  }
}