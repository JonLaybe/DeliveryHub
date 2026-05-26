import { FC, useEffect, useState, useRef, useCallback } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import type { Message } from "../../models/chat-service/Message";
import type { ChatWindowProps } from "../../models/chat-service/ChatWindowProps";
import { getMessagesForConversationAsync } from "../../services/chat-service/ChatService";
import { useChatSignalR } from "../../services/chat-service/SignalRService";
import "./ChatWindow.scss";

const ChatWindow: FC<ChatWindowProps> = ({ conversation, currentUserId }) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const { connection, joinConversation, sendMessage } = useChatSignalR();
  const location = useLocation();
  const navigate = useNavigate();
  
  const productName = (location.state as any)?.productName ?? "";
  const conversationName = (location.state as any)?.conversationName ?? conversation?.name ?? "Чат";
  const isOnline = (location.state as any)?.isOnline ?? conversation?.isOnline ?? false;
  
  const [newMessage, setNewMessage] = useState(() => {
    return productName ? `Здравствуйте, меня заинтересовал ваш товар: ${productName}` : "";
  });
  
  const hasSentGreetingRef = useRef(false);
  const messagesContainerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement | null>(null);
  const isUserScrollingRef = useRef(false);
  const scrollTimeoutRef = useRef<NodeJS.Timeout>();

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
    if (!conversation) return;

    const fetchMessages = async () => {
      try {
        const msgs = await getMessagesForConversationAsync(conversation.id, currentUserId);
        setMessages(msgs);
        setTimeout(() => {
          scrollToBottom("auto");
        }, 100);
      } catch (err) {
        console.error("Ошибка при получении сообщений:", err);
      }
    };

    fetchMessages();
    
    isUserScrollingRef.current = false;
    if (scrollTimeoutRef.current) {
      clearTimeout(scrollTimeoutRef.current);
    }
  }, [conversation, currentUserId, scrollToBottom]);

  useEffect(() => {
    if (messages.length > 0) {
      scrollToBottom("smooth");
    }
  }, [messages, scrollToBottom]);

  useEffect(() => {
    if (productName && messages.length > 0 && !hasSentGreetingRef.current) {
      const greetingMessage = `Здравствуйте, меня заинтересовал ваш товар: ${productName}`;
      const alreadySent = messages.some(msg => msg.text === greetingMessage);
      
      if (alreadySent) {
        setNewMessage("");
        hasSentGreetingRef.current = true;
        navigate(location.pathname, { replace: true, state: {} });
      }
    }
  }, [productName, messages, location.pathname, navigate]);

  useEffect(() => {
    if (!conversation) return;

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
  }, [conversation, currentUserId, connection, joinConversation]);

  const handleSend = useCallback(async () => {
    if (!newMessage.trim()) return;

    const textToSend = newMessage;
    setNewMessage("");
    hasSentGreetingRef.current = true;

    navigate(location.pathname, { replace: true, state: {} });

    try {
      await sendMessage(conversation.id, currentUserId, textToSend);
      inputRef.current?.focus();
    } catch (error) {
      console.error("Ошибка при отправке:", error);
    }
  }, [newMessage, conversation, currentUserId, sendMessage, navigate, location.pathname]);

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

  return (
    <div className="chat-window">
      <div className="chat-header">
        <div className="chat-header-avatar">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
            <circle cx="12" cy="7" r="4" />
          </svg>
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
        <button onClick={handleSend}>
          Отправить
        </button>
      </div>
    </div>
  );
};

export default ChatWindow;