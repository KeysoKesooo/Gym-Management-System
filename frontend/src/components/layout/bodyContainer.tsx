"use client";
import { useSidebar } from "@/context/sidebarContext";

export function BodyContainer({ children }: { children: React.ReactNode }) {
  const { open } = useSidebar();

  return (
    <main
      className="min-h-screen bg-gray-50 p-6 flex flex-col gap-4 transition-all duration-300"
      style={{ marginLeft: open ? 256 : 64 }} // sidebar width in px
    >
      {children}
    </main>
  );
}
