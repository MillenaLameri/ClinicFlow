export type Specialty = {
  id: string;
  name: string;
  isActive: boolean;
  createdAtUtc: string;
};

export type CreateSpecialtyRequest = {
  name: string;
};

export type UpdateSpecialtyRequest = {
  name: string;
};
