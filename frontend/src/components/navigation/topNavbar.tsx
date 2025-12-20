"use client";

import { useSidebar } from "@/context/sidebarContext";

interface TopNavbarProps {
  user: { name: string; role: string };
}

export function TopNavbar({ user }: TopNavbarProps) {
  const { open } = useSidebar();

  return (
    <nav
      className="fixed top-0 right-0 h-16 bg-white border-b shadow-sm flex items-center justify-between px-4 transition-all duration-300 z-10"
      style={{ 
        left: open ? 256 : 64, // match Sidebar width
        width: `calc(100% - ${open ? 256 : 64}px)` // fill remaining space
      }}
    >
      <div>
        <h2 className="text-lg font-semibold">Welcome, {user.name}</h2>
        <p className="text-gray-500 text-sm">{user.role}</p>
      </div>
      {/* You can add buttons / logout here */}
    </nav>
  );
}
