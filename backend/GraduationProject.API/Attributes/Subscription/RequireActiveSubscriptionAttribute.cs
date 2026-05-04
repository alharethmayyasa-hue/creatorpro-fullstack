using System;

namespace GraduationProject.API.Attributes.Subscription
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireActiveSubscriptionAttribute : Attribute
    {
    }
}