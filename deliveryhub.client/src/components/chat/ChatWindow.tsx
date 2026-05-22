import { useEffect, useState, useRef } from "react";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { Message } from "../../models/chat-service/Message";
import { getMessagesForConversationAsync } from "../../services/chat-service/ChatService";
import "./ChatWindow.scss";
import { v4 as uuidv4 } from "uuid";

interface ChatWindowProps {
  conversation: Conversation;
}

const ChatWindow: FC<ChatWindowProps> = ({ conversation, currentUserId }) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const didFetchRef = useRef(false);

  useEffect(() => {
    if (!conversation) return;
	if (didFetchRef.current) return;
    didFetchRef.current = true;
	
    const fetchMessages = async () => {
      try {
        const msgs = await getMessagesForConversationAsync(conversation.id, currentUserId);
		console.log("Messages for UI:", msgs);
        setMessages(msgs);
      } catch (err) {
        console.error("Ошибка при получении сообщений:", err);
      }
    };

    fetchMessages();
  }, [conversation]);

  const handleSend = () => {
    if (!newMessage.trim()) return;

    const message: Message = {
      id: uuidv4(),
      senderName: "Вы",
      text: newMessage,
      createdAt: new Date().toISOString(),
    };

    setMessages([...messages, message]);
    setNewMessage("");
  };

  return (
    <div className="chat-window">
      <div className="messages">
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