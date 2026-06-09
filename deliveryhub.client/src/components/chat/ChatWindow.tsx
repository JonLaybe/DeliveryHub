import { FC, useEffect, useState, useRef, useCallback } from "react";
import { useLocation } from "react-router-dom";
import type { Message } from "../../models/chat-service/Message";
import type { ChatWindowProps } from "../../models/chat-service/ChatWindowProps";
import { getMessagesForConversationAsync } from "../../services/chat-service/ChatService";
import { useChatSignalR } from "../../services/chat-service/SignalRService";
import "./ChatWindow.scss";

const ChatWindow: FC<ChatWindowProps> = ({ conversation, currentUserId, sellerPhoto }) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const { connection, joinConversation, sendMessage } = useChatSignalR();
  const location = useLocation();
  
  const productName = (location.state as any)?.productName ?? "";
  const conversationName = (location.state as any)?.conversationName ?? conversation?.name ?? "Чат";
  const isOnline = (location.state as any)?.isOnline ?? false;
  
  const [newMessage, setNewMessage] = useState<string>("");
  const [isLoading, setIsLoading] = useState(true);
  const hasSentResetEvent = useRef(false); // Флаг для предотвращения множественных событий
  
  const messagesContainerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement | null>(null);
  const isUserScrollingRef = useRef(false);
  const scrollTimeoutRef = useRef<NodeJS.Timeout>();
  const hasCheckedHistoryRef = useRef(false);

  const scrollToBottom = useCallback((behavior: ScrollBehavior = "smooth") => {
    if (messagesContainerRef.current && !isUserScrollingRef.current) {
      messagesContainerRef.current.scrollTo({
        top: messagesContainerRef.current.scrollHeight,
        behavior: behavior
      });
    }
  }, []);

  const handleScroll = useCallback(() => {
    if (messagesContainerRef.current) {
      const { scrollTop, scrollHeight, clientHeight } = messagesContainerRef.current;
      const isAtBottom = scrollHeight - scrollTop - clientHeight < 50;
      
      if (!isAtBottom) {
        isUserScrollingRef.current = true;
        
        if (scrollTimeoutRef.current) {
          clearTimeout(scrollTimeoutRef.current);
        }
        scrollTimeoutRef.current = setTimeout(() => {
          isUserScrollingRef.current = false;
        }, 3000);
      } else {
        isUserScrollingRef.current = false;
      }
    }
  }, []);

  useEffect(() => {
    if (conversation?.id && !hasSentResetEvent.current) {
      console.log("Chat opened, resetting unread count for:", conversation.id);
      window.dispatchEvent(new CustomEvent('chat:resetUnread', { 
        detail: { conversationId: conversation.id }
      }));
      hasSentResetEvent.current = true;
    }
  }, [conversation?.id]);

  useEffect(() => {
    if (!conversation?.id) return;

    const fetchMessages = async () => {
      setIsLoading(true);
      try {
        const msgs = await getMessagesForConversationAsync(conversation.id);
        setMessages(msgs);
        
        if (productName && !hasCheckedHistoryRef.current) {
          const hasUserMessage = msgs.some(msg => msg.senderName === "Вы");
          
          if (!hasUserMessage) {
            const greetingMessage = `Здравствуйте, меня заинтересовал ваш товар: ${productName}`;
            setNewMessage(greetingMessage);
          } else {
            setNewMessage("");
          }
          hasCheckedHistoryRef.current = true;
        }
        
        setTimeout(() => {
          scrollToBottom("auto");
        }, 100);
      } catch (err) {
        console.error("Ошибка при получении сообщений:", err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchMessages();
    
    isUserScrollingRef.current = false;
    if (scrollTimeoutRef.current) {
      clearTimeout(scrollTimeoutRef.current);
    }
  }, [conversation?.id, currentUserId, scrollToBottom, productName]);

  useEffect(() => {
    if (messages.length > 0 && !isLoading) {
      scrollToBottom("smooth");
    }
  }, [messages, scrollToBottom, isLoading]);

  useEffect(() => {
    if (!conversation?.id) return;

    const setup = async () => {
      await joinConversation(conversation.id);

      const handleReceiveMessage = (messageDto: any) => {
        const message: Message = {
          id: messageDto.id,
          senderName: messageDto.senderId === currentUserId ? "Вы" : "Собеседник",
          text: messageDto.text,
          createdAt: messageDto.createdAt,
        };

        setMessages((prev) => {
          if (prev.some(m => m.id === message.id)) return prev;
          return [...prev, message];
        });
      };

      connection.on("ReceiveMessage", handleReceiveMessage);

      return () => {
        connection.off("ReceiveMessage", handleReceiveMessage);
      };
    };

    setup();
  }, [conversation?.id, currentUserId, connection, joinConversation]);

  const handleSend = useCallback(async () => {
    if (!newMessage.trim()) return;

    const textToSend = newMessage;
    setNewMessage("");

    try {
      await sendMessage(conversation.id, textToSend);
      
      window.dispatchEvent(new CustomEvent('chat:updateList'));
      
      inputRef.current?.focus();
    } catch (error) {
      console.error("Ошибка при отправке:", error);
    }
  }, [newMessage, conversation?.id, currentUserId, sendMessage]);

  const handleKeyDown = useCallback((e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  }, [handleSend]);

  const formatTime = useCallback((date?: Date) => {
    if (!date) return "";
    return new Date(date).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
  }, []);

  if (isLoading) {
    return (
      <div className="chat-window">
        <div className="chat-header">
          <div className="chat-header-avatar">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
              <circle cx="12" cy="7" r="4" />
            </svg>
          </div>
          <div className="chat-header-info">
            <h3>{conversationName}</h3>
          </div>
        </div>
        <div className="messages loading">
          <div className="loading-message">Загрузка сообщений...</div>
        </div>
        <div className="input-area">
          <input disabled placeholder="Загрузка..." value="" />
          <button disabled>Отправить</button>
        </div>
      </div>
    );
  }

  return (
    <div className="chat-window">
      <div className="chat-header">
        <div className="chat-header-avatar">
          {sellerPhoto ? (
            <img src={sellerPhoto} alt={conversationName} />
          ) : (
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
              <circle cx="12" cy="7" r="4" />
            </svg>
          )}
          {isOnline && <div className="online-indicator-header"></div>}
        </div>
        <div className="chat-header-info">
          <h3>{conversationName}</h3>
          <p className="online-status">{isOnline ? "В сети" : "Не в сети"}</p>
        </div>
      </div>

      <div 
        className="messages" 
        ref={messagesContainerRef}
        onScroll={handleScroll}
      >
        {messages.map((m, index) => (
          <div
            key={m.id || index}
            className={`message ${m.senderName === "Вы" ? "outgoing" : "incoming"}`}
          >
            <div className="message-bubble">
              <div className="message-text">{m.text}</div>
              <div className="message-time">
                {formatTime(m.createdAt)}
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="input-area">
        <input
          ref={inputRef}
          type="text"
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Введите сообщение..."
        />
        <button onClick={handleSend} disabled={!newMessage.trim()}>
          Отправить
        </button>
      </div>
    </div>
  );
};

export default ChatWindow;