export interface GiftRequestPayload {
  FriendPlayerId: string;
  ResourceTypeId: string;
  ResourceValue: number;
}

export interface GiftResponsePayload {
  Success: boolean;
  ResourceTypeId: string;
  NewTotal: number;
  Message: string;
}