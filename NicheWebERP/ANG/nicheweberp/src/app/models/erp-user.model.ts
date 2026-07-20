export interface ErpUser {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  roleId: number;
  roleName: string;
  locationId: string | null;
  isActive: boolean;
  lastLoginAt: string | null;
}

export interface CreateErpUserRequest {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  roleId: number;
  locationId: string | null;
}

export interface UpdateErpUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  roleId: number;
  locationId: string | null;
  isActive: boolean;
}

export interface ErpRole {
  id: number;
  name: string;
  description: string | null;
}
