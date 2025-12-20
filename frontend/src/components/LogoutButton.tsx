"use client";

import { useRouter } from "next/navigation";
import { FiLogOut } from "react-icons/fi";

interface LogoutButtonProps {
  iconOnly?: boolean;
  className?: string;
}

export default function LogoutButton({ iconOnly = false, className }: LogoutButtonProps) {
  const router = useRouter();

  const handleLogout = async () => {
    try {
      const token = localStorage.getItem("token");

      if (token) {
        const res = await fetch("http://localhost:5279/api/auth/logout", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) console.error("Logout request failed:", await res.text());
      }

      localStorage.removeItem("token");
      localStorage.removeItem("user");

      router.push("/login");
    } catch (error) {
      console.error("Logout failed:", error);
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      router.push("/login");
    }
  };

  return iconOnly ? (
    <button
      onClick={handleLogout}
      title="Logout"
      className={`p-2 text-gray-700 hover:text-white hover:bg-red-500 rounded transition ${className}`}
    >
      <FiLogOut size={20} />
    </button>
  ) : (
    <button
      onClick={handleLogout}
      className={`w-full flex items-center justify-start gap-2 px-4 py-2 bg-red-500 hover:bg-red-600 text-white rounded font-semibold transition ${className}`}
    >
      <FiLogOut size={18} />
      Logout
    </button>
  );
}
