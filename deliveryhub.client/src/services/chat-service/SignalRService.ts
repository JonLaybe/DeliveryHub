import { useSignalR } from "../../context/SignalRContext";

export function useChatSignalR() {
  const { connection, ensureConnected } = useSignalR();

  const joinConversation = async (conversationId: string) => {
    await ensureConnected();
    await connection.invoke("JoinConversation", conversationId);
  };

	const sendMessage = async (conversationId: string, text: string) => {
    await ensureConnected();
    await connection.invoke("SendMessage", conversationId, text);
  };

  return {
    connection,
    joinConversation,
    sendMessage,
  };
}