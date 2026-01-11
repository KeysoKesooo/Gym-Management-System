// ✅ define the shape of your user (same as in RequireAuth)
export type User = {
  id: number;
  name: string;
  email: string;
  role: string;
};

export type CreateUser = {
  name: string;
  email: string;
  password: string;
  role?: string;
};

export type UpdateUser = {
  name?: string;
  email?: string;
  password?: string;
  role?: string;
};

export type Attendance = {
  id: number;
  userId: number;
  user: User;
  checkIn: string; // backend sends DateTime → frontend gets string
  checkOut?: string | null;
};

export type MemberContentProps = {
  user: User;
};
