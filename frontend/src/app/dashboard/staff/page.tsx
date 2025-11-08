"use client";

import RequireAuth from "@/components/RequireAuth";
import LogoutButton from "@/components/LogoutButton";

export default function StaffDashboard() {
  return (
    <RequireAuth allowedRoles={["staff", "admin"]}>
      <div className="p-6">
        <h1 className="text-2xl font-bold mb-4">Staff Dashboard</h1>
        <p>Welcome, you are logged in as Staff.</p>
        <LogoutButton />
      </div>
    </RequireAuth>
  );
}
