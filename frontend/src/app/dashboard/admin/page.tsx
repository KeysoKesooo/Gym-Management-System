"use client";

import RequireAuth from "@/components/RequireAuth";
import LogoutButton from "@/components/LogoutButton";
import { User } from "@/types/IntUser";

export default function AdminDashboard() {
  return (
    <RequireAuth allowedRoles={["admin"]}>
      {(user: User) => (
        <div className="p-6">
          <h1 className="text-2xl font-bold mb-4">Admin Dashboard</h1>
          <p>
            Welcome, <span className="font-semibold">{user.name}</span> (
            {user.role})
          </p>
          <LogoutButton />
        </div>
      )}
    </RequireAuth>
  );
}
