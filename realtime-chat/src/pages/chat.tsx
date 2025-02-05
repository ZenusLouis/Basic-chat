import { useContext, useState } from "react";
import { AuthContext } from "@/context/AuthContext";
import Messenger from "@/components/Messenger";

export default function ChatPage() {
  const { user } = useContext(AuthContext);

  return user ? <Messenger user={user} /> : <p>Please log in</p>;
}
