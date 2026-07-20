export interface CurrentUser {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  roleId: number;
  roleName: string;
  locationId: string | null;
  companyId: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  user: CurrentUser;
}
