import { createContext, useContext, useEffect, useRef, } from "react";
import * as signalR from "@microsoft/signalr";
import { CHAT_URL, CHAT_HUB_URL } from "../constants/EndpointConstants";
import { getAccessToken } from "../services/auth-service/AuthService";

type SignalRContextType = {
  connection: signalR.HubConnection;
  ensureConnected: () => Promise<void>;
};

const SignalRContext = createContext<SignalRContextType | null>(null);

export const useSignalR = () => {
  const ctx = useContext(SignalRContext);
  if (!ctx) throw new Error("SignalRProvider missing");
  return ctx;
};

export const SignalRProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const connectionRef = useRef(
    new signalR.HubConnectionBuilder()
      .withUrl(`${CHAT_URL}${CHAT_HUB_URL}`, {
        accessTokenFactory: () => getAccessToken() || ""
      })
      .withAutomaticReconnect()
      .build()
  );

  let startPromise: Promise<void> | null = null;

  const ensureConnected = async () => {
    const connection = connectionRef.current;

    if (connection.state === signalR.HubConnectionState.Connected) {
      return;
    }
	
	try {
      if (!startPromise) {
        startPromise = connection.start();
      }
      await startPromise;
	} catch (error) {
        console.error("SignalR connection failed:", error);
        startPromise = null;
        throw error;
	  }
  };

  useEffect(() => {
    const connection = connectionRef.current;

    connection.onclose(() => {
      console.log("SignalR disconnected");
	  startPromise = null;
    });

    connection.onreconnecting(() => {
      console.log("SignalR reconnecting");
    });

    connection.onreconnected(() => {
      console.log("SignalR reconnected");
    });
  }, []);

  return (
    <SignalRContext.Provider
      value={{
        connection: connectionRef.current,
        ensureConnected,
      }}
    >
      {children}
    </SignalRContext.Provider>
  );
};