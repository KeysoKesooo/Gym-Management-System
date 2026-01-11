"use client";

import { useEffect, useState } from "react";
import { User, CreateUser, UpdateUser } from "@/types/IntUser";
import { isValidEmail, isValidPassword } from "@/utils/validators";


const API_URL = "http://localhost:5279/api/user";

export default function useMember(token: string | null) {
  const [member, setMember] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch all members
  const fetchMember = async () => {
    if (!token) return;

    try {
      const res = await fetch(`${API_URL}/member`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error("Failed to fetch staff");

      const data: User[] = await res.json();
      setMember(data);
    } catch (err: any) {
      console.error(err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

const addMember = async (data: CreateUser) => {
  if (!token) return;

  // ✅ VALIDATION ONLY
  if (!isValidEmail(data.email)) {
    setError("Invalid email address");
    return;
  }

  const passwordError = isValidPassword(data.password);
  if (passwordError) {
    setError(passwordError);
    return;
  }

  try {
    const payload = { ...data, role: data.role ?? "member" };

    const res = await fetch(`${API_URL}/create`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(payload),
    });

    if (!res.ok) throw new Error(await res.text());

    const created: User = await res.json();
    setMember((prev) => [...prev, created]);
  } catch (err: any) {
    console.error("Add member failed:", err.message);
    setError(err.message);
  }
};


 const updateMember = async (id: number, data: UpdateUser) => {
  if (!token || id === undefined) return;

  // ✅ VALIDATION ONLY
  if (data.email && !isValidEmail(data.email)) {
    setError("Invalid email address");
    return;
  }

  if (data.password) {
    const passwordError = isValidPassword(data.password);
    if (passwordError) {
      setError(passwordError);
      return;
    }
  }

  try {
    const cleanData = Object.fromEntries(
      Object.entries(data).filter(([_, v]) => v !== undefined && v !== "")
    );

    const res = await fetch(`${API_URL}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(cleanData),
    });

    if (!res.ok) throw new Error(await res.text());

    const updated: User = await res.json();
    setMember((prev) => prev.map((m) => (m.id === id ? updated : m)));
  } catch (err: any) {
    console.error("Update member failed:", err.message);
    setError(err.message);
  }
};


  // Delete member
  const deleteMember = async (id: number) => {
    if (!token || id === undefined) return;

    try {
      const res = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error(await res.text());
      setMember((prev) => prev.filter((m) => m.id !== id));
    } catch (err: any) {
      console.error("Delete member failed:", err.message);
      setError(err.message);
    }
  };

  useEffect(() => {
    fetchMember();
  }, []);

  return {
    member,
    loading,
    error,
    fetchMember,
    addMember,
    updateMember,
    deleteMember,
  };
}
