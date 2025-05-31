export interface LoginRequestPayload {
  DeviceId: string;
}

export interface LoginResponsePayload {
  Success: boolean;
  Error: string | undefined
  PlayerId: string;
  Stats: Stat[]
}

export interface Stat {
  ResourceTypeId: string;
  ResourceName: string;
  Total: number;
}