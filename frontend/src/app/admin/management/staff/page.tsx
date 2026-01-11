"use client";

import { useState } from "react";
import RequireAuth from "@/components/RequireAuth";
import { SidebarProvider } from "@/context/sidebarContext";
import { Sidebar } from "@/components/navigation/sidebar";
import { TopNavbar } from "@/components/navigation/topNavbar";
import { BodyContainer } from "@/components/layout/bodyContainer";
import { Footer } from "@/components/layout/footer";

import useStaff from "@/hooks/useStaffManagement";
import { User, CreateUser, UpdateUser } from "@/types/IntUser";

export default function StaffManagement() {
  return (
    <RequireAuth allowedRoles={["admin"]}>
      {(user) => (
        <SidebarProvider>
          <AdminContent user={user} />
        </SidebarProvider>
      )}
    </RequireAuth>
  );
}

function AdminContent({ user }: { user: any }) {
  const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;

  const { staff, loading, error, addStaff, updateStaff, deleteStaff } = useStaff(token);

  const [showAdd, setShowAdd] = useState(false);
  const [editing, setEditing] = useState<User | null>(null);

  const [createData, setCreateData] = useState<CreateUser>({
    name: "",
    email: "",
    password: "",
    role: "staff",
  });

  const [updateData, setUpdateData] = useState<UpdateUser>({});

  return (
    <>
      <Sidebar role={user.role as "admin" | "staff" | "member"} />
      <TopNavbar user={user} />
      <BodyContainer>
        <h1 className="text-2xl font-bold mb-4">Staff Management</h1>

        <button className="mb-4 px-4 py-2 bg-blue-600 text-white rounded" onClick={() => setShowAdd(true)}>
          Add Staff
        </button>

        {loading ? (
          <p>Loading staff...</p>
        ) : error ? (
          <p className="text-red-500">{error}</p>
        ) : (
          <table className="min-w-full border">
            <thead>
              <tr className="bg-gray-100">
                <th className="p-2 border">Name</th>
                <th className="p-2 border">Email</th>
                <th className="p-2 border">Role</th>
                <th className="p-2 border" style={{ width: 160 }}>
                  Actions
                </th>
              </tr>
            </thead>
            <tbody>
              {staff.length === 0 && (
                <tr>
                  <td colSpan={4} className="text-center p-2">
                    No staff found
                  </td>
                </tr>
              )}
              {staff.map((s) => (
                <tr key={s.id}>
                  <td className="p-2 border">{s.name}</td>
                  <td className="p-2 border">{s.email}</td>
                  <td className="p-2 border">{s.role}</td>
                  <td className="p-2 border">
                    <button
                      className="px-2 py-1 bg-yellow-500 text-white rounded"
                      onClick={() => {
                        setEditing(s);
                        setUpdateData({ name: s.name, email: s.email, role: s.role });
                      }}
                    >
                      Edit
                    </button>
                    <button
                      className="ml-2 px-2 py-1 bg-red-600 text-white rounded"
                      onClick={() => s.id && deleteStaff(s.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {/* Add Modal */}
        {showAdd && (
          <div style={modalStyle}>
            <h3 className="text-lg font-bold mb-2">Add Staff</h3>
            <input
              className="border p-1 mb-2 w-full"
              placeholder="Name"
              value={createData.name}
              onChange={(e) => setCreateData({ ...createData, name: e.target.value })}
            />
            <input
              className="border p-1 mb-2 w-full"
              placeholder="Email"
              value={createData.email}
              onChange={(e) => setCreateData({ ...createData, email: e.target.value })}
            />
            <input
              type="password"
              className="border p-1 mb-2 w-full"
              placeholder="Password"
              value={createData.password}
              onChange={(e) => setCreateData({ ...createData, password: e.target.value })}
            />
            <div className="flex gap-2 mt-2">
              <button
                className="px-3 py-1 bg-green-600 text-white rounded"
                onClick={() => {
                  addStaff(createData);
                  setShowAdd(false);
                  setCreateData({ name: "", email: "", password: "", role: "staff" });
                }}
              >
                Save
              </button>
              <button className="px-3 py-1 bg-gray-400 text-white rounded" onClick={() => setShowAdd(false)}>
                Cancel
              </button>
            </div>
          </div>
        )}

        {/* Edit Modal */}
        {editing && (
          <div style={modalStyle}>
            <h3 className="text-lg font-bold mb-2">Edit Staff</h3>
            <input
              className="border p-1 mb-2 w-full"
              defaultValue={editing.name}
              onChange={(e) => setUpdateData({ ...updateData, name: e.target.value })}
            />
            <input
              className="border p-1 mb-2 w-full"
              defaultValue={editing.email}
              onChange={(e) => setUpdateData({ ...updateData, email: e.target.value })}
            />
            <input
              type="password"
              className="border p-1 mb-2 w-full"
              placeholder="New password (optional)"
              onChange={(e) => setUpdateData({ ...updateData, password: e.target.value })}
            />
            <div className="flex gap-2 mt-2">
              <button
                className="px-3 py-1 bg-green-600 text-white rounded"
                onClick={() => {
                  if (editing.id) updateStaff(editing.id, updateData);
                  setEditing(null);
                }}
              >
                Update
              </button>
              <button className="px-3 py-1 bg-gray-400 text-white rounded" onClick={() => setEditing(null)}>
                Cancel
              </button>
            </div>
          </div>
        )}
      </BodyContainer>
      <Footer />
    </>
  );
}

// Simple modal style
const modalStyle: React.CSSProperties = {
  position: "fixed",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  background: "#fff",
  padding: 24,
  borderRadius: 8,
  boxShadow: "0 4px 12px rgba(0,0,0,0.2)",
  display: "flex",
  flexDirection: "column",
  gap: 8,
  zIndex: 1000,
  minWidth: 320,
};
