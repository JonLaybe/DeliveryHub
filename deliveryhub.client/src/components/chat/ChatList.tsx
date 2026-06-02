import { FC, useEffect, useState, useRef } from "react";
import { useNavigate, useParams } from "react-router-dom";
import type { Conversation } from "../../models/chat-service/Conversation";
import { getUserConversationsAsync } from "../../services/chat-service/ChatService";
import "./ChatList.scss";

const ChatList = () => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const didFetchRef = useRef(false);
  const navigate = useNavigate();
  const { conversationId } = useParams();

  useEffect(() => {
    if (conversationId) {
      setSelectedId(conversationId);
    }
  }, [conversationId]);

  const fetchConversations = async () => {
    try {
      const data = await getUserConversationsAsync();
      console.log("Conversations for UI:", data);
      setConversations(data);
    } catch (err) {
      console.error("Ошибка при получении чатов:", err);
    }
  };

  useEffect(() => {
    if (didFetchRef.current) return;
    didFetchRef.current = true;
    fetchConversations();
  }, []);

  useEffect(() => {
    const handleUpdateList = () => {
      fetchConversations();
    };

    window.addEventListener('chat:updateList', handleUpdateList);

    return () => {
      window.removeEventListener('chat:updateList', handleUpdateList);
    };
  }, []);

  useEffect(() => {
    const handleResetUnread = (event: CustomEvent) => {
      const { conversationId } = event.detail;
      if (conversationId) {
        console.log("Resetting unread count for:", conversationId);
        setConversations(prev => prev.map(conv => 
          conv.id === conversationId ? { ...conv, unreadMessagesCount: 0 } : conv
        ));
      }
    };

    window.addEventListener('chat:resetUnread', handleResetUnread as EventListener);

    return () => {
      window.removeEventListener('chat:resetUnread', handleResetUnread as EventListener);
    };
  }, []);

  const handleSelect = (conv: Conversation) => {
    setSelectedId(conv.id);
    
    setConversations(prev => prev.map(c => 
      c.id === conv.id ? { ...c, unreadMessagesCount: 0 } : c
    ));
    
    navigate(`/chat/${conv.id}`, { 
      state: { 
        conversationName: conv.name,
        isOnline: conv.isOnline 
      } 
    });
  };

  const formatMessageTime = (lastMessageAt?: string) => {
    if (!lastMessageAt) return "";
    
    try {
      const date = new Date(lastMessageAt);
      if (isNaN(date.getTime())) {
        return "";
      }
      
      const now = new Date();
      const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
      const messageDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
      
      if (today.getTime() === messageDate.getTime()) {
        return date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
      }
      
      const yesterday = new Date(today);
      yesterday.setDate(yesterday.getDate() - 1);
      if (yesterday.getTime() === messageDate.getTime()) {
        return "Вчера";
      }
      
      const weekDays = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
      if (now.getTime() - date.getTime() < 7 * 24 * 60 * 60 * 1000) {
        return weekDays[date.getDay()];
      }
      
      return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'short' });
    } catch (error) {
      console.error("Error formatting date:", error);
      return "";
    }
  };

  return (
    <div className="chat-list">
      <div className="chat-list-header">
        <h2>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
          </svg>
          Сообщения
        </h2>
        <div className="chat-count">{conversations.length} чатов</div>
      </div>
      
      <div className="chat-items">
        {conversations.length === 0 ? (
          <div className="empty-chats">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
              <path d="M20 2H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h14l4 4V4c0-1.1-.9-2-2-2z" />
              <path d="M8 10h8M12 14V6" strokeLinecap="round" />
            </svg>
            <p>У вас пока нет чатов</p>
          </div>
        ) : (
          conversations.map((conv, index) => (
            <div
              key={conv.id || index}
              className={`chat-item ${selectedId === conv.id ? "selected" : ""} ${conv.unreadMessagesCount && conv.unreadMessagesCount > 0 ? "has-unread" : ""}`}
              onClick={() => handleSelect(conv)}
            >
              <div className="chat-avatar">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                  <circle cx="12" cy="7" r="4" />
                </svg>
                {conv.isOnline && <div className="online-indicator"></div>}
              </div>
              <div className="chat-info">
                <div className="chat-name">
                  <span>{conv.name || `Чат ${index + 1}`}</span>
                  {conv.lastMessage && conv.lastMessageAt && (
                    <span className="chat-time">
                      {formatMessageTime(conv.lastMessageAt)}
                    </span>
                  )}
                </div>
                <span className="chat-last-message">
                  {conv.lastMessage || "Нет сообщений"}
                </span>
              </div>
              {conv.unreadMessagesCount !== undefined && conv.unreadMessagesCount !== null && conv.unreadMessagesCount > 0 && (
                <div className="unread-badge">{conv.unreadMessagesCount}</div>
              )}
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default ChatList;