import { Navigate } from "react-router";
import { useAuth } from "@/features/auth/hooks/useAuth";

export function DashboardRedirect() {
  const { user, isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (user?.roles.includes("Admin")) {
    return <Navigate to="/dashboard/admin" replace />;
  }

  if (user?.roles.includes("Doctor")) {
    return <Navigate to="/dashboard/doctor" replace />;
  }

  if (user?.roles.includes("Patient")) {
    return <Navigate to="/dashboard/patient" replace />;
  }

  return <Navigate to="/login" replace />;
}
