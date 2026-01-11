export const getStaff = async (): Promise<any[]> => {
  const res = await fetch("http://localhost:5279/api/user/staff");
  if (!res.ok) throw new Error("Failed to fetch staff");
  const data = await res.json();
  return Array.isArray(data) ? data : [];
};

export const createStaff = async (data: any) => {
  const res = await fetch("http://localhost:5279/api/user", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to create staff");
  return res.json();
};

export const updateStaff = async (id: number, data: any) => {
  const res = await fetch(`http://localhost:5279/api/user/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to update staff");
  return res.json();
};

export const deleteStaff = async (id: number) => {
  const res = await fetch(`http://localhost:5279/api/user/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to delete staff");
  return true;
};
