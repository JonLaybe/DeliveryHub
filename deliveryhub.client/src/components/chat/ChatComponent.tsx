import { useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import ChatList from "./ChatList";
import ChatWindow from "./ChatWindow";
import type { Conversation } from "../../models/chat-service/Conversation";
import { getCurrentUser } from "../../services/auth-service/AuthService";
import { getUserConversationsAsync } from "../../services/chat-service/ChatService";
import "./ChatComponent.scss";

const ChatComponent = () => {
  const { conversationId } = useParams();
  const location = useLocation();
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);
  const [selectedConversation, setSelectedConversation] = useState<Conversation | null>(null);
  const [isLoadingConversation, setIsLoadingConversation] = useState(false);

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
  
  useEffect(() => {
    if (!conversationId || !currentUserId) return;
    
    const loadConversation = async () => {
      setIsLoadingConversation(true);
      try {
        const conversations = await getUserConversationsAsync();
        const found = conversations.find(c => c.id === conversationId);
        
        if (found) {
          setSelectedConversation(found);
        } else {
          setSelectedConversation(null);
          console.warn("Conversation not found:", conversationId);
        }
      } catch (error) {
        console.error("Failed to load conversation:", error);
        setSelectedConversation(null);
      } finally {
        setIsLoadingConversation(false);
      }
    };
    
    loadConversation();
  }, [conversationId, currentUserId]);
  
  if (!currentUserId || isLoadingConversation) {
    return <div className="chat-container">Загрузка...</div>;
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
            sellerPhoto={selectedConversation.sellerPhoto}
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