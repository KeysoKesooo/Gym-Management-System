"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { FiUser, FiUsers, FiSettings, FiMenu, FiLogOut, FiDatabase } from "react-icons/fi";
import { useSidebar } from "@/context/sidebarContext";
import LogoutButton from "@/components/LogoutButton";
import React from "react";
import { FileChartLine } from "lucide-react";
import { FaCashRegister } from "react-icons/fa";

type Role = "admin" | "staff" | "member";

interface SidebarLink {
  href: string;
  label: string;
  icon: React.ReactNode;
}

const linksByRole: Record<Role, SidebarLink[]> = {
  admin: [
    { href: "/admin/dashboard", label: "Dashboard", icon: <FileChartLine /> },
    { href: "/admin/management/staff", label: "Staff Management", icon: <FiUser /> },
    { href: "/admin/management/member", label: "Member Management", icon: <FiUsers /> },
    { href: "/logs/logs", label: "Logs", icon: <FiDatabase /> },
    { href: "/setting/account", label: "Account Settings", icon: <FiSettings /> },

  ],
  staff: [
    { href: "/staff/dashboard", label: "Staff Management", icon: <FileChartLine /> },
    { href: "/staff/pos", label: "Staff Management", icon: <FaCashRegister /> },
    { href: "/admin/management/member", label: "Member Management", icon: <FiUsers /> },
    { href: "/setting/account", label: "Account Settings", icon: <FiSettings /> },

  ],
  member: [
    { href: "/member/dashboard", label: "Dashboard", icon: <FiUsers /> },
    { href: "/setting/account", label: "Account Settings", icon: <FiSettings /> },
    
  ],
};

interface SidebarProps {
  role: Role;
}

export function Sidebar({ role }: SidebarProps) {
  const path = usePathname();
  const { open, toggle } = useSidebar();
  const links = linksByRole[role] || [];

  return (
    <aside
      className={`bg-white border-r shadow-sm fixed top-0 left-0 h-full transition-all duration-300 flex flex-col justify-between ${
        open ? "w-64" : "w-16"
      }`}
    >
      {/* Top Section: Toggle + Links */}
      <div>
        <button
          className="p-2 m-2 bg-emerald-600 text-white rounded"
          onClick={toggle}
        >
          <FiMenu />
        </button>

        <nav className="mt-4 flex flex-col gap-2">
          {links.map((link) => {
            const active = path === link.href;
            return (
              <Link
                key={link.href}
                href={link.href}
                className={`flex items-center gap-2 p-2 rounded hover:bg-emerald-100 transition ${
                  active ? "bg-emerald-600 text-white" : "text-gray-700"
                }`}
              >
                {link.icon}
                {open && <span>{link.label}</span>}
              </Link>
            );
          })}
        </nav>
      </div>

      {/* Bottom Section: Logout */}
      <div className="p-4 mt-auto">
        <LogoutButton iconOnly={!open} />
      </div>

    </aside>
  );
}
