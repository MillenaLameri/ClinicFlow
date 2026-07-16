export type DoctorSpecialty = {
  id: string;
  name: string;
};

export type Doctor = {
  id: string;
  fullName: string;
  crmNumber: string;
  crmState: string;
  email: string;
  phone: string | null;
  specialty: DoctorSpecialty;
  isActive: boolean;
  createdAtUtc: string;
};

export type GetDoctorsParams = {
  page: number;
  pageSize: number;
  search?: string;
  specialtyId?: string;
  includeInactive?: boolean;
};

export type CreateDoctorRequest = {
  fullName: string;
  crmNumber: string;
  crmState: string;
  email: string;
  phone: string | null;
  specialtyId: string;
};

export type UpdateDoctorRequest = {
  fullName: string;
  email: string;
  phone: string | null;
  specialtyId: string;
};