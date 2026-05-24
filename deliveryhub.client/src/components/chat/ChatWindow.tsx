import { FC, useEffect, useState, useRef } from "react";
import { useLocation } from "react-router-dom";
import type { Message } from "../../models/chat-service/Message";
import type { ChatWindowProps } from "../../models/chat-service/ChatWindowProps";
import { getMessagesForConversationAsync } from "../../services/chat-service/ChatService";
import { useChatSignalR } from "../../services/chat-service/SignalRService";

import "./ChatWindow.scss";

const ChatWindow: FC<ChatWindowProps> = ({ conversation, currentUserId, }) => {
  const [messages, setMessages] = useState<Message[]>([]);

  const { connection, joinConversation, sendMessage } = useChatSignalR();
  const location = useLocation();
  
  const productName = (location.state as any)?.productName ?? "";
  
  const [newMessage, setNewMessage] = useState(() => {
    return productName
      ? `Здравствуйте, меня заинтересовал ваш товар: ${productName}`
      : "";
  });

  const messagesRef = useRef<HTMLDivElement | null>(null);

  const inputRef = useRef<HTMLInputElement | null>(null);

  

  const scrollToBottom = () => {
    messagesRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    if (!conversation) return;

    setMessages([]);

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
    messagesRef.current?.scrollIntoView({
	  behavior: "smooth",
      block: "end",
    });
  }, [messages]);

  useEffect(() => {
    if (!conversation) return;

    const setup = async () => {
      await joinConversation(conversation.id);

      connection.off("ReceiveMessage");

      connection.on("ReceiveMessage", (messageDto) => {
        const message: Message = {
          id: messageDto.id,
          senderName:
            messageDto.senderId === currentUserId
              ? "Вы"
              : "Собеседник",
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

  return (
    <div className="chat-window">
      <div className="messages">
        {messages.map((m) => (
          <div
            key={m.id}
            className={`message ${
              m.senderName === "Вы"
                ? "outgoing"
                : "incoming"
            }`}
          >
            <span className="text">{m.text}</span>
          </div>
        ))}
        <div ref={messagesRef} />
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