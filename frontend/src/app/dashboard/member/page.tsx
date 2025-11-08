"use client";


import { useEffect, useState } from "react";
import RequireAuth from "@/components/RequireAuth";
import LogoutButton from "@/components/LogoutButton";
import { Attendance, MemberContentProps } from "@/types/IntUser";

export default function MemberDashboard() {
  return (
    <RequireAuth allowedRoles={["member", "admin"]}>
      {(user) => <MemberContent user={user} />}
    </RequireAuth>
  );
}

function MemberContent({ user }: MemberContentProps) {
  const [attendance, setAttendance] = useState<Attendance[]>([]);

  useEffect(() => {
    const token = localStorage.getItem("token");

    const fetchAttendance = async () => {
      try {
        const res = await fetch("http://localhost:5279/api/attendance/my", {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) return;
        const data: Attendance[] = await res.json();
        setAttendance(data);
      } catch (err) {
        console.error("Failed to fetch attendance", err);
      }
    };

    fetchAttendance();
  }, []);

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <h1 className="text-3xl font-bold">Member Dashboard</h1>
      <p className="mt-2">
        Welcome, {user.name} ({user.role})
      </p>

      <div className="mt-6 bg-white p-6 rounded-lg shadow">
        <h2 className="text-xl font-semibold mb-4">📅 Attendance History</h2>
        {attendance.length > 0 ? (
          <table className="w-full border-collapse">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2 border">Check-in</th>
                <th className="p-2 border">Check-out</th>
              </tr>
            </thead>
            <tbody>
              {attendance.map((a) => (
                <tr key={a.id}>
                  <td className="p-2 border">
                    {new Date(a.checkIn).toLocaleString()}
                  </td>
                  <td className="p-2 border">
                    {a.checkOut
                      ? new Date(a.checkOut).toLocaleString()
                      : "Still inside"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No attendance records found.</p>
        )}
      </div>

      <LogoutButton />
    </div>
  );
}
