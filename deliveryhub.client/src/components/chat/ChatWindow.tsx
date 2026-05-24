import { FC, useEffect, useState, useRef } from "react";
import type { Message } from "../../models/chat-service/Message";
import type { ChatWindowProps } from "../../models/chat-service/ChatWindowProps";
import { getMessagesForConversationAsync } from "../../services/chat-service/ChatService";
import { useChatSignalR } from "../../services/chat-service/SignalRService";

import "./ChatWindow.scss";

const ChatWindow: FC<ChatWindowProps> = ({ conversation, currentUserId, }) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState("");

  const { connection, joinConversation, sendMessage } = useChatSignalR();

  const messagesRef = useRef<HTMLDivElement | null>(null);

  const scrollToBottom = () => {
    const el = messagesRef.current;
    if (!el) return;

    el.scrollTop = el.scrollHeight;
  };

  useEffect(() => {
    if (!conversation) return;

    setMessages([]);

    const fetchMessages = async () => {
      try {
        const msgs = await getMessagesForConversationAsync(conversation.id, currentUserId);
        setMessages(msgs);
      } catch (err) {
        console.error(err);
      }
    };

    fetchMessages();
  }, [conversation, currentUserId]);

  useEffect(() => {
    scrollToBottom();
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
  };

  return (
    <div className="chat-window">
      <div className="messages" ref={messagesRef}>
        {messages.map((m) => (
          <div
            key={m.id}
            className={`message ${m.senderName === "Вы" ? "outgoing" : "incoming"}`}
          >
            <span className="text">{m.text}</span>
          </div>
        ))}
      </div>

      <div className="input-area">
        <input
          type="text"
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          placeholder="Введите сообщение..."
        />
        <button onClick={handleSend}>Отправить</button>
      </div>
    </div>
  );
};

export default ChatWindow;