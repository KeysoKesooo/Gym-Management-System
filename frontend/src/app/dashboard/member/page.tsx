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
  const [loading, setLoading] = useState(true);
  const [isCheckedIn, setIsCheckedIn] = useState(false);

  const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;

  const fetchAttendance = async () => {
    try {
      const res = await fetch("http://localhost:5279/api/attendance/my", {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) return;

      const data: Attendance[] = await res.json();
      setAttendance(data);
      // check if user currently checked in (last record has no checkout)
      const last = data[data.length - 1];
      setIsCheckedIn(last && !last.checkOut);
    } catch (err) {
      console.error("Failed to fetch attendance", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAttendance();
  }, []);

  const handleAttendance = async () => {
    try {
      const res = await fetch("http://localhost:5279/api/attendance/check", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (res.ok) {
        await fetchAttendance();
      } else {
        console.error("Failed to update attendance");
      }
    } catch (err) {
      console.error("Error during attendance update", err);
    }
  };

  if (loading) return <p className="p-6 text-gray-600">Loading...</p>;

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold">Member Dashboard</h1>
          <p className="mt-2 text-gray-600">
            Welcome, {user.name} ({user.role})
          </p>
        </div>
        <LogoutButton />
      </div>

      <div className="mt-6 bg-white p-6 rounded-lg shadow">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">📅 Attendance History</h2>
          <button
            onClick={handleAttendance}
            className={`px-4 py-2 rounded-lg font-semibold transition-colors ${
              isCheckedIn
                ? "bg-red-500 hover:bg-red-600 text-white"
                : "bg-green-500 hover:bg-green-600 text-white"
            }`}
          >
            {isCheckedIn ? "Check Out" : "Check In"}
          </button>
        </div>

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
          <p className="text-gray-600">No attendance records found.</p>
        )}
      </div>
    </div>
  );
}
