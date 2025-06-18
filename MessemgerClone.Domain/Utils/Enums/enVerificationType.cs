using MessengerClone.Utilities.Constants;

namespace MessengerClone.Domain.Utils.Enums;

public enum enVerificationType
{
    EmailConfirmation = 1,
    PhoneConfirmation,
    PasswordReset,
    PasswordChange,
    EmailChange,
    PhoneChange
}
