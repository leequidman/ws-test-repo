namespace Common.Models;

public enum EventType
{
    LoginInit,
    LoginSuccessful,
    LoginFailed,

    UpdateResourceInit,     
    UpdateResourceSuccess,
    UpdateResourceFailure,

    SendGiftInit,
    SendGiftSuccess,
    SendGiftFailure,
    GiftReceived
}