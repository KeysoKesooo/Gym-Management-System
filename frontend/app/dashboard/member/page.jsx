"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import RequireAuth from "../../components/RequireAuth";
import LogoutButton from "../../components/LogoutButton";

export default function MemberDashboard() {
  const router = useRouter();
  const [user, setUser] = useState(null);
  const [attendance, setAttendance] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");

    if (!token || role !== "Member") {
      router.push("/login");
      return;
    }

    setUser({ role });

    const fetchAttendance = async () => {
      try {
        const res = await fetch("http://localhost:5279/api/attendance/my", {
          headers: { Authorization: `Bearer ${token}` },
        });

        if (!res.ok) return;
        const data = await res.json();
        setAttendance(data);
      } catch (err) {
        console.error("Failed to fetch attendance", err);
      }
    };

    fetchAttendance();
  }, [router]);


  return (
    <RequireAuth allowedRoles={["Member", "Admin"]}>
    <div className="min-h-screen bg-gray-50 p-6">
      <h1 className="text-3xl font-bold">Member Dashboard</h1>
      {user && <p className="mt-2">Welcome, {user.role}</p>}

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
    </RequireAuth>
  );
}
