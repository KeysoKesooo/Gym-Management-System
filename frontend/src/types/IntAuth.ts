import React, { useEffect, useState, ReactNode } from "react";
import { User } from "./IntUser";
// Define the component props
export interface RequireAuthProps {
  allowedRoles?: string[];
  children: ReactNode | ((user: User) => ReactNode);
}
