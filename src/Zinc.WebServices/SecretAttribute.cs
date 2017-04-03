using System;

namespace Zinc.WebServices
{
    /// <summary>
    /// Indicates that the current property is a secret, and should
    /// not be journaled.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property )]
    public class SecretAttribute : Attribute
    {
    }


    /// <summary>
    /// Indicates that the current class has a secret property and that
    /// prior to journaling, a deep clone should be made and the secret
    /// removed.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
    public class HasSecretAttribute : Attribute
    {
    }
}
