import type { Metadata } from "next";
import "./globals.css";
import Link from "next/link";

export const metadata: Metadata = {
  title: "Gym Management",
  description: "Gym Management System",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body suppressHydrationWarning>
        <nav className="bg-gray-800 text-white px-6 py-4 flex justify-between">
          <Link href="/" className="font-bold">
            Gym System
          </Link>
          <div className="space-x-4">
            {/* ✅ Fix: use /login not ./pages/login */}
            <Link href="/login" className="hover:underline">
              Login
            </Link>
          </div>
        </nav>
        <main className="p-6">{children}</main>
      </body>
    </html>
  );
}
