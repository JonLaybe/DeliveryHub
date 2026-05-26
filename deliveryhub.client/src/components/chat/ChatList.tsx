import { FC, useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { ChatListProps } from "../../models/chat-service/ChatListProps";
import { getUserConversationsAsync } from "../../services/chat-service/ChatService";
import "./ChatList.scss";

const ChatList: FC<ChatListProps> = ({ userId, }) => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const didFetchRef = useRef(false);

  useEffect(() => {
    if (!userId) return;
    if (didFetchRef.current) return;

    didFetchRef.current = true;

    const fetchConversations = async () => {
      try {
        const data = await getUserConversationsAsync(userId);
		console.log("Conversations for UI:", data);
        setConversations(data);
      } catch (err) {
        console.error("Ошибка при получении чатов:", err);
      }
    };
	
    fetchConversations();
  }, [userId]);

  const navigate = useNavigate();

  const handleSelect = (conv: Conversation) => {
    setSelectedId(conv.id);
    navigate(`/chat/${conv.id}`);
  };

  return (
    <div className="chat-list">
		{conversations.map((conv, index) => (
			<div
				key={conv.id || index} // гарантируем уникальный ключ
				className={`chat-item ${selectedId === conv.id ? "selected" : ""}`}
				onClick={() => handleSelect(conv)}
			>
				<span className="chat-name">{conv.name}</span>
				{conv.lastMessage && <span className="chat-last-message">{conv.lastMessage}</span>}
			</div>
		))}
    </div>
  );
};

export default ChatList;