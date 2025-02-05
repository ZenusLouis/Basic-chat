import { useState, useEffect } from "react";
import UserList from "./UserList";
import Chat from "./Chat";

export default function Messenger({ user }) {
  const [selectedUser, setSelectedUser] = useState(null);

  return (
    <div className="flex h-screen">
      <UserList user={user} setSelectedUser={setSelectedUser} />
      {selectedUser ? <Chat user={user} selectedUser={selectedUser} /> : <p className="m-auto">Select a user to chat</p>}
    </div>
  );
}
