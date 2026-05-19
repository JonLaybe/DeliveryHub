import { useState } from "react";
import ChatList from "./ChatList";
import type { Conversation } from "../../models/chat-service/Conversation";
import type { ChatListProps } from "../../models/chat-service/ChatListProps";

const ChatPage = () => {
  const [selectedConversation, setSelectedConversation] = useState<Conversation | null>(null);

  return (
    <div style={{ display: "flex", height: "100vh" }}>
      <ChatList
        userId="c8e4a03b-960e-4874-80b0-fea30a90fc7b"
        onSelectConversation={setSelectedConversation}
      />
      <div style={{ flex: 1, display: "flex", justifyContent: "center", alignItems: "center" }}>
        {selectedConversation ? (
          <div>Окно чата: {selectedConversation.name}</div>
        ) : (
          <div>Выберите чат</div>
        )}
      </div>
    </div>
  );
};

export default ChatPage;