export interface CustomerProfile {
  id: number;
  name: string;
  email: string;
  phone: string;
}

export interface CustomerAuthResponse {
  token: string;
  expiresAtUtc: string;
  customer: CustomerProfile;
}

export interface CustomerRegisterPayload {
  name: string;
  email: string;
  phone: string;
  password: string;
}

export interface CustomerLoginPayload {
  email: string;
  password: string;
}
