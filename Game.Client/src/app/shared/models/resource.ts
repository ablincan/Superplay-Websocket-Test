export interface ResourceRequestPayload {
  ResourceTypeId: string;
  Delta: number;
}

export interface ResourceResponsePayload {
  Success: boolean;
  ResourceTypeId: string;
  NewTotal: number;
}