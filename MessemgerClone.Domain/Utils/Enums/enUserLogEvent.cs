namespace MessengerClone.Domain.Utils.Enums;

public enum enUserLogEvent
{
    LoginSuccess = 1,
    LoginFailed,
    Logout,
    ProfileUpdated,
    AccountDeactivated,
    AccountReactivated,
    AccountDeleted,
    EmailChanged,
    PasswordChanged,
    PasswordReset,
    PhoneChanged,
    EmailConfirmed,
    PhoneConfirmed,
    TwoFactorEnabled,
    TokenRefreshed,
    AccountLockedout,
    AccountUnLocked,
    MfaEnabled,
    MfaDisabled
}

