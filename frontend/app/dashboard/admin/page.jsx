"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";

export default function AdminDashboard() {
  const [loading, setLoading] = useState(true);
  const [authorized, setAuthorized] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const userStr = localStorage.getItem("user");
    if (!userStr) {
      router.push("/login?error=not-logged-in");
      return;
    }

    try {
      const user = JSON.parse(userStr);
      if (user.role.toLowerCase() === "admin") {
        setAuthorized(true);
      } else {
        router.push("/unauthorized"); // 🚫 custom "Oops you can't go to that page"
      }
    } catch (err) {
      console.error("Error parsing user:", err);
      router.push("/login?error=session-invalid");
    } finally {
      setLoading(false);
    }
  }, [router]);

  if (loading) {
    return <p>Checking access...</p>;
  }

  if (!authorized) return null; // prevent flicker

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold">Admin Dashboard</h1>
      <p>Welcome, you are logged in as Admin.</p>
    </div>
  );
}
