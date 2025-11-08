// ✅ define the shape of your user (same as in RequireAuth)
export interface User {
  id?: number;
  name: string;
  email: string;
  role: string;
}

export interface Attendance {
  id: number;
  userId: number;
  user: User;
  checkIn: string;     // backend sends DateTime → frontend gets string
  checkOut?: string | null;
}

export interface MemberContentProps {
  user: User;
}