"use client";

import React, { useEffect, useState, ReactNode } from "react";
import { useRouter } from "next/navigation";
import { User } from "@/types/IntUser";
import { RequireAuthProps } from "@/types/IntAuth";


export default function RequireAuth({
  children,
  allowedRoles = [],
}: RequireAuthProps) {
  const [loading, setLoading] = useState(true);
  const [authorized, setAuthorized] = useState(false);
  const [user, setUser] = useState<User | null>(null);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    const userStr = localStorage.getItem("user");

    if (!token || !userStr) {
      router.replace("/login");
      return;
    }

    try {
      const userObj: User = JSON.parse(userStr);
      const userRole = userObj.role?.toLowerCase();
      const allowed = allowedRoles.map((r) => r.toLowerCase());

      if (!allowed.includes(userRole)) {
        router.replace("/unauthorized");
      } else {
        setUser(userObj);
        setAuthorized(true);
      }
    } catch (err) {
      console.error("Auth parse error", err);
      router.replace("/login");
    } finally {
      setLoading(false);
    }
  }, [router, allowedRoles]);

  if (loading) return <div className="text-center mt-10">Checking access...</div>;
  if (!authorized) return null;

  // ✅ If children is a render function, pass the user
  if (typeof children === "function") {
    return <>{(children as (user: User) => ReactNode)(user!)}</>;
  }

  // ✅ Otherwise, just render children directly
  return <>{children}</>;
}
