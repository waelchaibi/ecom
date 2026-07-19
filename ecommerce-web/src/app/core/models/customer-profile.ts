export interface CustomerProfile {
  id: number;
  name: string;
  email: string;
  phone: string;
  identityCard: string;
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
  identityCard: string;
  password: string;
}

export interface CustomerLoginPayload {
  email: string;
  password: string;
}

export interface UpdateCustomerProfilePayload {
  name: string;
  phone: string;
  identityCard: string;
}
