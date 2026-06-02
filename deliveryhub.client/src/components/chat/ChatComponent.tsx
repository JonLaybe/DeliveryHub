import { useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import ChatList from "./ChatList";
import ChatWindow from "./ChatWindow";
import type { Conversation } from "../../models/chat-service/Conversation";
import { getCurrentUser } from "../../services/auth-service/AuthService";
import "./ChatComponent.scss";

const ChatComponent = () => {
  const { conversationId } = useParams();
  const location = useLocation();
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);

  useEffect(() => { 
    window.scrollTo(0, 0);
  }, [location.pathname]);
  
  useEffect(() => {
    const loadUser = async () => {
      try {
        const user = await getCurrentUser();
        setCurrentUserId(user.id);
      } catch (error) {
        console.error("Failed to load current user:", error);
      }
    };
    loadUser();
  }, []);
  
  const productName = (location.state as any)?.productName ?? "";

  const selectedConversation = conversationId
    ? ({ id: conversationId } as Conversation)
    : null;
	
  if (!currentUserId) {
    return <div className="chat-container">Загрузка пользователя...</div>;
  }

  return (
    <div className="chat-container">
      <ChatList/>
      
      <div className="chat-main">
        {selectedConversation ? (
          <ChatWindow
            key={selectedConversation.id}
            conversation={selectedConversation}
            currentUserId={currentUserId}
          />
        ) : (
          <div className="chat-placeholder">
            <div className="placeholder-content">
              <svg className="placeholder-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
              </svg>
              <h3>Выберите чат</h3>
              <p>Вам есть, что обсудить!</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ChatComponent;