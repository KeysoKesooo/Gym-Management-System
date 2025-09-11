"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";

export default function RequireAuth({ children, allowedRoles }) {
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");

    if (!token || !role || !allowedRoles.includes(role)) {
      // Redirect with message
      localStorage.setItem("authError", "Oops, you can't go to that page");
      router.replace("/login");
    } else {
      setLoading(false);
    }
  }, [router, allowedRoles]);
  
  if (loading) {
    return <div className="text-center mt-10">Checking access...</div>;
  }

  return children;
}
