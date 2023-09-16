namespace Common.Models;

public enum EventType
{
    InitLogin,
    LoginSuccessful,
    LoginFailed,

    InitUpdateResource,     
    UpdateResourceSuccess,
    UpdateResourceFailure,


    SendGift
}