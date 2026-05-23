import { useState } from "react";
import ChatList from "./ChatList";
import ChatWindow from "./ChatWindow";
import type { Conversation } from "../../models/chat-service/Conversation";

const ChatComponent = () => {
  const [selectedConversation, setSelectedConversation] = useState<Conversation | null>(null);
  const currentUserId = "c8e4a03b-960e-4874-80b0-fea30a90fc7b";

  return (
    <div style={{ display: "flex", height: "100vh" }}>
      <ChatList
        userId={currentUserId}
        onSelectConversation={setSelectedConversation}
      />
      <div style={{ flex: 1, display: "flex" }}>
        {selectedConversation ? (
          <ChatWindow
		    key={selectedConversation.id}
            conversation={selectedConversation}
            currentUserId={currentUserId}
          />
        ) : (
          <div style={{ margin: "auto" }}>Выберите чат</div>
        )}
      </div>
    </div>
  );
};

export default ChatComponent;