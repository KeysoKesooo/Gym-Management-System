"use client";

import RequireAuth from "@/components/RequireAuth";
import LogoutButton from "@/components/LogoutButton";
import { SidebarProvider } from "@/context/sidebarContext";
import { Sidebar } from "@/components/navigation/sidebar";
import { TopNavbar } from "@/components/navigation/topNavbar";
import { BodyContainer } from "@/components/layout/bodyContainer";
import { Footer } from "@/components/layout/footer";
import useAttendance from "@/hooks/useAttendance";
import { MemberContentProps } from "@/types/IntUser";

export default function MemberDashboard() {
  return (
    <RequireAuth allowedRoles={["member", "admin"]}>
      {(user) => (
        <SidebarProvider>
          <MemberContent user={user} />
        </SidebarProvider>
      )}
    </RequireAuth>
  );
}

function MemberContent({ user }: MemberContentProps) {
  const token =
    typeof window !== "undefined" ? localStorage.getItem("token") : null;

  const { attendance, loading, isCheckedIn, handleAttendance } =
    useAttendance(token);

  return (
    <>
      {/* Sidebar and TopNavbar flow */}
      <Sidebar role={user.role as  "member"} /> 
      <TopNavbar user={user} />

      <BodyContainer>
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold">Member Dashboard</h1>
            <p className="mt-2 text-gray-600">
              Welcome, {user.name} ({user.role})
            </p>
          </div>
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

          {loading ? (
            <p className="text-gray-600">Loading...</p>
          ) : attendance.length > 0 ? (
            <table className="w-full border-collapse">
              <thead>
                <tr className="bg-gray-200 text-left">
                  <th className="p-2 border">Check-in</th>
                  <th className="p-2 border">Check-out</th>
                </tr>
              </thead>
              <tbody>
                {attendance.map((a, index) => (
                  <tr
                    key={a.id || `${index}-${new Date(a.checkIn).getTime()}`}
                  >
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
      </BodyContainer>
      <Footer />
    </>
  );
}
