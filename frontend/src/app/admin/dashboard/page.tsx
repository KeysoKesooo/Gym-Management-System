"use client";

import RequireAuth from "@/components/RequireAuth";
import { SidebarProvider } from "@/context/sidebarContext"; // ✅ import provider
import { Sidebar } from "@/components/navigation/sidebar";
import { TopNavbar } from "@/components/navigation/topNavbar";
import { BodyContainer } from "@/components/layout/bodyContainer";
import { Footer } from "@/components/layout/footer";

export default function AdminDashboard() {
  return (
    <RequireAuth allowedRoles={["admin"]}>
      {(user) => (
        // ✅ Wrap everything that uses useSidebar in SidebarProvider
        <SidebarProvider>
          <AdminContent user={user} />
        </SidebarProvider>
      )}
    </RequireAuth>
  );
}

function AdminContent({ user }: { user: any }) {
  return (
    <>
      <Sidebar role={user.role as "admin" | "staff" | "member"} />
      <TopNavbar user={user} />
      <BodyContainer>
        <h1 className="text-2xl font-bold mb-4">Admin Dashboard</h1>
        <p>Admin overview, stats, and user management will go here.</p>
      </BodyContainer>
      <Footer />
    </>
  );
}
