import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

export default function Chat({ user, selectedUser }) {
  const [messages, setMessages] = useState([]);
  const [message, setMessage] = useState("");
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    if (!selectedUser) return;

    const token = localStorage.getItem("token");

    fetch("http://localhost:5286/api/chat/messages", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ sender: user, receiver: selectedUser }),
    })
      .then((res) => res.json())
      .then((data) => setMessages(Array.isArray(data) ? data : []))
      .catch((err) => console.error("❌ Error fetching messages:", err));

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5286/chatHub", {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    newConnection
      .start()
      .then(() => {
        console.log("✅ Connected to SignalR");
        newConnection.on("ReceiveMessage", (sender, receiver, msg) => {
          if (receiver === user || sender === user) {
            setMessages((prev) => [...prev, { sender, receiver, content: msg }]);
          }
        });
      })
      .catch((err) => console.error("SignalR Connection Error:", err));

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, [selectedUser]);

  const sendMessage = () => {
    if (!message.trim()) return;

    const newMessage = { sender: user, receiver: selectedUser, content: message };

    fetch("http://localhost:5286/api/chat/send", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify(newMessage),
    })
      .then(() => {
        setMessage("");
      })
      .catch((err) => console.error("❌ Error sending message:", err));
  };

  return (
    <div className="flex-1 flex flex-col h-screen">
      <div className="p-4 bg-blue-600 text-white text-center font-bold">{selectedUser}</div>
      <div className="flex-1 p-4 overflow-y-auto">
        {messages.map((msg, index) => (
          <div
            key={index}
            className={`p-2 my-1 max-w-xs rounded-lg ${
              msg.sender === user ? "bg-blue-500 text-white self-end ml-auto" : "bg-gray-300 text-black"
            }`}
          >
            {msg.sender === user ? "You" : msg.sender}: {msg.content}
          </div>
        ))}
      </div>
      <div className="p-4 bg-gray-100 flex">
        <input
          type="text"
          className="flex-1 p-2 border rounded"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />
        <button className="ml-2 bg-blue-500 text-white p-2 rounded" onClick={sendMessage}>
          Send
        </button>
      </div>
    </div>
  );
}
