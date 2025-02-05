import { useState, useEffect } from "react";

export default function UserList({ user, setSelectedUser }) {
  const [users, setUsers] = useState([]);

  useEffect(() => {
    fetch("http://localhost:5286/api/users", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify({ currentUser: user }),
    })
      .then((res) => res.json())
      .then((data) => setUsers(Array.isArray(data) ? data : []))
      .catch((err) => console.error("❌ Error fetching users:", err));
  }, []);

  return (
    <div className="w-1/4 p-4 border-r">
      <h2 className="text-lg font-bold mb-4">Users</h2>
      {users.map((u) => (
        <button
          key={u.username}
          className="block w-full p-2 border rounded mb-2 bg-gray-200 hover:bg-gray-300"
          onClick={() => setSelectedUser(u.username)}
        >
          {u.username}
        </button>
      ))}
    </div>
  );
}
