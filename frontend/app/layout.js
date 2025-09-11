// app/layout.js
import "./globals.css";

export const metadata = {
  title: "Gym Management",
  description: "Gym Management System",
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <head />
      <body>
        <nav className="bg-gray-800 text-white px-6 py-4 flex justify-between">
          <a href="/" className="font-bold">Gym System</a>
          <div className="space-x-4">
            <a href="/login" className="hover:underline">Login</a>
          </div>
        </nav>

        <main className="p-6">{children}</main>
      </body>
    </html>
  );
}
