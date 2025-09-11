"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import RequireAuth from "../../components/RequireAuth";
import LogoutButton from "../../components/LogoutButton";

export default function StaffDashboard() {
  const router = useRouter();
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");

    if (!token || role !== "Staff") {
      router.push("/login");
    } else {
      setUser({ role });
    }
  }, [router]);

  return (
    <RequireAuth allowedRoles={["Staff", "Admin"]}>
    <div className="min-h-screen bg-gray-50 p-6">
      <h1 className="text-3xl font-bold">Staff Dashboard</h1>
      {user && <p className="mt-2">Welcome, {user.role}</p>}

      <div className="grid md:grid-cols-2 gap-6 mt-6">
        <div className="p-6 bg-white shadow rounded-lg">📝 Check-in Members</div>
        <div className="p-6 bg-white shadow rounded-lg">📦 Manage Attendance</div>
      </div>
      <LogoutButton />
    </div>
    </RequireAuth>
  );
}
