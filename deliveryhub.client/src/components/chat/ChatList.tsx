import { FC, useEffect, useState, useRef } from "react";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { ChatListProps } from "../../models/chat-service/ChatListProps";
import { getUserConversationsAsync } from "../../services/chat-service/ChatService";
import "./ChatList.scss";

const ChatList: FC<ChatListProps> = ({ userId, onSelectConversation }) => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null); // <-- выбранный чат
  const didFetchRef = useRef(false);

  useEffect(() => {
    if (!userId) return;
    if (didFetchRef.current) return;

    didFetchRef.current = true;

    const fetchConversations = async () => {
      try {
        const data = await getUserConversationsAsync(userId);
        console.log("API Response:", data);
      } catch (err) {
        console.error("Ошибка при получении чатов:", err);
      }
    };

    // Заглушка
    const fakeConversations: Conversation[] = [
      { id: "1", name: "Иван Петров", lastMessage: "Привет!" },
      { id: "2", name: "Мария Сидорова", lastMessage: "До встречи завтра" },
      { id: "3", name: "Александр Любимов" },
    ];
    setConversations(fakeConversations);

    fetchConversations();
  }, [userId]);

  const handleSelect = (conv: Conversation) => {
    setSelectedId(conv.id);       // <-- сохраняем выбранный чат
    onSelectConversation(conv);   // <-- передаем в родителя
  };

  return (
    <div className="chat-list">
      {conversations.map((conv) => (
        <div
          key={conv.id}
          className={`chat-item ${selectedId === conv.id ? "selected" : ""}`} // <-- выделение
          onClick={() => handleSelect(conv)}
        >
          <span className="chat-name">{conv.name}</span>
          {conv.lastMessage && (
            <span className="chat-last-message">{conv.lastMessage}</span>
          )}
        </div>
      ))}
    </div>
  );
};

export default ChatList;