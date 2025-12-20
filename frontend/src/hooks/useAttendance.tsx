"use client";

import { useEffect, useState } from "react";
import { Attendance } from "@/types/IntUser";

export default function useAttendance(token: string | null) {
  const [attendance, setAttendance] = useState<Attendance[]>([]);
  const [loading, setLoading] = useState(true);
  const [isCheckedIn, setIsCheckedIn] = useState(false);

  // Fetch attendance history
  const fetchAttendance = async () => {
    if (!token) return;

    try {
      const res = await fetch("http://localhost:5279/api/attendance/my", {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) return;

      const data: Attendance[] = await res.json();

      // Sort newest first
      const sorted = data.sort(
        (a, b) => new Date(b.checkIn).getTime() - new Date(a.checkIn).getTime()
      );
      setAttendance(sorted);

      // Check if inside
      const last = sorted[0];
      setIsCheckedIn(last && !last.checkOut);
    } catch (err) {
      console.error("Failed to fetch attendance", err);
    } finally {
      setLoading(false);
    }
  };

  // Handle check-in / check-out
  const handleAttendance = async () => {
    if (!token) return;

    try {
      const res = await fetch("http://localhost:5279/api/attendance/check", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (res.ok) {
        await fetchAttendance();
      } else {
        console.error("Failed to update attendance");
      }
    } catch (err) {
      console.error("Error during attendance update", err);
    }
  };

  // Load on mount
  useEffect(() => {
    fetchAttendance();
  }, []);

  return {
    attendance,
    loading,
    isCheckedIn,
    handleAttendance,
  };
}
