import { useSignalR } from "../../context/SignalRContext";

export function useChatSignalR() {
  const { connection, ensureConnected } = useSignalR();

  const joinConversation = async (conversationId: string) => {
    await ensureConnected();
    await connection.invoke("JoinConversation", conversationId);
  };

  const sendMessage = async (conversationId: string, senderId: string, text: string) => {
    await ensureConnected();
    await connection.invoke("SendMessage", conversationId, senderId, text);
  };

  return {
    connection,
    joinConversation,
    sendMessage,
  };
}