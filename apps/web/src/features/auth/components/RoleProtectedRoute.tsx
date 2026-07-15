import type { ReactNode } from "react";
import { Navigate } from "react-router";
import { useAuth } from "@/features/auth/hooks/useAuth";

import type { UserRole } from "@/features/auth/types/auth.types";

type RoleProtectedRouteProps = {
  children: ReactNode;
  allowedRoles: UserRole[];
};

export function RoleProtectedRoute({
  children,
  allowedRoles,
}: RoleProtectedRouteProps) {
  const { user, isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const hasPermission =
    user?.roles.some((role) => allowedRoles.includes(role)) ?? false;

  if (!hasPermission) {
    return <Navigate to="/dashboard" replace />;
  }

  return children;
}
