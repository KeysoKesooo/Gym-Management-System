export function Footer() {
  return (
    <footer className="w-full bg-gray-100 p-4 border-t mt-auto">
      <p className="text-center text-gray-600 text-sm">
        © {new Date().getFullYear()} Gym Management System. All rights reserved.
      </p>
    </footer>
  );
}
