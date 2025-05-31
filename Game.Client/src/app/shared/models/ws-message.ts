export interface WsMessage<T = any> {
  MessageType: string;
  Payload: T;
}