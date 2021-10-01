
using System;

namespace SignalRSwaggerGen.Attributes
{
    /// <summary>
    /// An Attribute request Body Example 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SignalRRequestBodyAttribute : Attribute {

        /// <summary>
        /// The Requested Type
        /// </summary>
        public Type RequestType { get; }

        /// <summary>
        /// Either this Attribute required or not 
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Example Provider if any 
        /// </summary>
        public string Description { get; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestType"> The type of The body request</param>
        /// <param name="isRequired">indicate if it's mandatory or not</param>
        /// <param name="description">The Body request Description</param>
        public SignalRRequestBodyAttribute(Type requestType, string description  ,bool isRequired = false )
        {
            RequestType = requestType;
            IsRequired = isRequired;
            Description = description;
        }
    }
}
