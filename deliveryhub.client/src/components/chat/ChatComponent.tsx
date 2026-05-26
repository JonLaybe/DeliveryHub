import { useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import ChatList from "./ChatList";
import ChatWindow from "./ChatWindow";
import type { Conversation } from "../../models/chat-service/Conversation";

const ChatComponent = () => {
  const { conversationId } = useParams();
  const location = useLocation();

  useEffect(() => { 
    window.scrollTo(0, 0);
  }, [location.pathname]);
  
  const productName = (location.state as any)?.productName ?? "";

  const currentUserId = "c8e4a03b-960e-4874-80b0-fea30a90fc7b";

  const selectedConversation = conversationId
    ? ({ id: conversationId } as Conversation)
    : null;

  return (
    <div style={{ display: "flex", height: "100vh" }}>
      <ChatList userId={currentUserId} />

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