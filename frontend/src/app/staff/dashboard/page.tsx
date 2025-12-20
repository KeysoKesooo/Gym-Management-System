"use client";

import RequireAuth from "@/components/RequireAuth";
import { SidebarProvider } from "@/context/sidebarContext";
import { Sidebar } from "@/components/navigation/sidebar";
import { TopNavbar } from "@/components/navigation/topNavbar";
import { BodyContainer } from "@/components/layout/bodyContainer";
import { Footer } from "@/components/layout/footer";

export default function StaffDashboard() {
  return (
    <RequireAuth allowedRoles={["staff", "admin"]}>
      {(user) => (
        // Wrap everything using useSidebar with SidebarProvider
        <SidebarProvider>
          <StaffContent user={user} />
        </SidebarProvider>
      )}
    </RequireAuth>
  );
}

function StaffContent({ user }: { user: any }) {
  return (
    <>
      <Sidebar role={user.role as "staff" } />
      <TopNavbar user={user} />
      <BodyContainer>
        <h1 className="text-2xl font-bold mb-4">Staff Dashboard</h1>
        <p>Staff overview, attendance management, and member monitoring will go here.</p>
        {/* Example: You can add a data table for member attendance */}
      </BodyContainer>
      <Footer />
    </>
  );
}
