import { FC, useEffect, useState, useRef, useLayoutEffect } from "react";
import { useLocation } from "react-router-dom";
import type { Message } from "../../models/chat-service/Message";
import type { ChatWindowProps } from "../../models/chat-service/ChatWindowProps";
import { getMessagesForConversationAsync } from "../../services/chat-service/ChatService";
import { useChatSignalR } from "../../services/chat-service/SignalRService";
import "./ChatWindow.scss";

const ChatWindow: FC<ChatWindowProps> = ({ conversation, currentUserId }) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const { connection, joinConversation, sendMessage } = useChatSignalR();
  const location = useLocation();
  
  const productName = (location.state as any)?.productName ?? "";
  const conversationName = (location.state as any)?.conversationName ?? conversation?.name ?? "Чат";
  
  const [newMessage, setNewMessage] = useState(() => {
    return productName
      ? `Здравствуйте, меня заинтересовал ваш товар: ${productName}`
      : "";
  });

  const messagesContainerRef = useRef<HTMLDivElement | null>(null);
  const inputRef = useRef<HTMLInputElement | null>(null);
  const prevMessagesLengthRef = useRef(0);

  const scrollToBottom = () => {
    if (messagesContainerRef.current) {
      messagesContainerRef.current.scrollTop = messagesContainerRef.current.scrollHeight;
    }
  };

  useLayoutEffect(() => {
    if (messages.length > prevMessagesLengthRef.current) {
      scrollToBottom();
    }
    prevMessagesLengthRef.current = messages.length;
  }, [messages]);

  useEffect(() => {
    if (!conversation) return;

    setMessages([]);
    prevMessagesLengthRef.current = 0;

    const fetchMessages = async () => {
      try {
        const msgs = await getMessagesForConversationAsync(conversation.id, currentUserId);
        setMessages(msgs);
      } catch (err) {
        console.error("Ошибка при получении сообщений:", err);
      }
    };

    fetchMessages();
  }, [conversation, currentUserId]);

  useEffect(() => {
    if (!conversation) return;

    const setup = async () => {
      await joinConversation(conversation.id);

      connection.off("ReceiveMessage");

      connection.on("ReceiveMessage", (messageDto) => {
        const message: Message = {
          id: messageDto.id,
          senderName: messageDto.senderId === currentUserId ? "Вы" : "Собеседник",
          text: messageDto.text,
          createdAt: messageDto.createdAt,
        };

        setMessages((prev) => [...prev, message]);
      });
    };

    setup();

    return () => {
      connection.off("ReceiveMessage");
    };
  }, [conversation, currentUserId]);

  const handleSend = async () => {
    if (!newMessage.trim()) return;

    const textToSend = newMessage;
    setNewMessage("");

    await sendMessage(conversation.id, currentUserId, textToSend);

    setTimeout(() => {
      inputRef.current?.focus();
    }, 0);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleSend();
    }
  };

  const formatTime = (date?: Date) => {
    if (!date) return "";
    return new Date(date).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
  };

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

      <div className="messages" ref={messagesContainerRef}>
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
        <button onClick={handleSend}>Отправить</button>
      </div>
    </div>
  );
};

export default ChatWindow;