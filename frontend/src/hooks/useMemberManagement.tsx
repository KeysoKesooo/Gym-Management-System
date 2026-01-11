"use client";

import { useEffect, useState } from "react";
import { User, CreateUser, UpdateUser } from "@/types/IntUser";

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

  // Add staff
  const addMember = async (data: CreateUser) => {
    if (!token) return;
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
      console.error("Add staff failed:", err.message);
      setError(err.message);
    }
  };

  // Update member (optional password)
  const updateMember = async (id: number, data: UpdateUser) => {
    if (!token || id === undefined) return;

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
      setMember((prev) => prev.map((s) => (s.id === id ? updated : s)));
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
      setMember((prev) => prev.filter((s) => s.id !== id));
    } catch (err: any) {
      console.error("Delete staff failed:", err.message);
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
