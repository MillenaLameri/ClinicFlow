import { Navigate, Route, Routes } from "react-router";
import { DashboardRedirect } from "@/features/auth/components/DashboardRedirect";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute";
import { RoleProtectedRoute } from "@/features/auth/components/RoleProtectedRoute";
import { LoginPage } from "@/pages/auth/LoginPage";
import { AdminDashboardPage } from "@/pages/dashboard/admin/AdminDashboardPage";
import { DoctorDashboardPage } from "@/pages/dashboard/doctor/DoctorDashboardPage";
import { PatientDashboardPage } from "@/pages/dashboard/patient/PatientDashboardPage";
import { DoctorsPage } from "@/pages/admin/doctors/DoctorsPage";
import { SpecialtiesPage } from "@/pages/admin/specialties/SpecialtiesPage";

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <DashboardRedirect />
          </ProtectedRoute>
        }
      />

      <Route
        path="/dashboard/admin"
        element={
          <RoleProtectedRoute allowedRoles={["Admin"]}>
            <AdminDashboardPage />
          </RoleProtectedRoute>
        }
      />

      <Route
        path="/dashboard/doctor"
        element={
          <RoleProtectedRoute allowedRoles={["Doctor"]}>
            <DoctorDashboardPage />
          </RoleProtectedRoute>
        }
      />

      <Route
        path="/dashboard/patient"
        element={
          <RoleProtectedRoute allowedRoles={["Patient"]}>
            <PatientDashboardPage />
          </RoleProtectedRoute>
        }
      />

      <Route
        path="/admin/doctors"
        element={
          <RoleProtectedRoute allowedRoles={["Admin"]}>
            <DoctorsPage />
          </RoleProtectedRoute>
        }
      />

      <Route path="/" element={<Navigate to="/dashboard" replace />} />

      <Route
        path="/admin/specialties"
        element={
          <RoleProtectedRoute allowedRoles={["Admin"]}>
            <SpecialtiesPage />
          </RoleProtectedRoute>
        }
      />

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
