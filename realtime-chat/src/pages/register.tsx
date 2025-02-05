import { useState } from "react";
import { useRouter } from "next/router";

export default function RegisterPage() {
  const router = useRouter();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleRegister = async () => {
    setError("");
    try {
      const res = await fetch("http://localhost:5286/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }), // 🔥 Sửa lại đây
      });

      if (res.ok) {
        router.push("/login");
      } else {
        const data = await res.json();
        setError(data.errors?.password?.[0] || "Registration failed");
      }
    } catch (err) {
      setError("Cannot connect to the server");
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-6 rounded shadow-md w-96">
        <h2 className="text-2xl font-bold mb-4">Register</h2>
        {error && <p className="text-red-500">{error}</p>}
        <input 
          type="text" 
          placeholder="Username" 
          className="w-full p-2 border rounded mb-2" 
          value={username} 
          onChange={(e) => setUsername(e.target.value)} 
        />
        <input 
          type="password" 
          placeholder="Password" 
          className="w-full p-2 border rounded mb-2" 
          value={password} 
          onChange={(e) => setPassword(e.target.value)} 
        />
        <button 
          className="w-full bg-green-500 text-white p-2 rounded hover:bg-green-600"
          onClick={handleRegister}
        >
          Register
        </button>
      </div>
    </div>
  );
}
