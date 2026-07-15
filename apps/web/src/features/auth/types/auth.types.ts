export type UserRole = "Admin" | "Doctor" | "Patient";

export type AuthenticatedUser = {
  id: string;
  fullName: string;
  email: string;
  roles: UserRole[];
  patientId: string | null;
  doctorId: string | null;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type AuthenticationResponse = {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  user: AuthenticatedUser;
};

export type AuthSession = {
  accessToken: string;
  refreshToken: string;
  user: AuthenticatedUser;
};
