using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.API.Authorization
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CheckPermissionAttribute : Attribute
    {
        public CheckPermissionAttribute(enPermission permission)
        {
            Permission = permission;
        }

        public enPermission Permission { get; }
    }
}
