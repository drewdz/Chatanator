using System.Runtime.Serialization;

namespace PE.Framework.Models
{
    #region Enumerations

    public enum ServiceResultStatus
    {
        Success,
        Failure,
        Warning,
        Error
    }

    #endregion Enumerations

    [DataContract]
    public class ServiceResult
    {
        #region Constructors

        public ServiceResult()
        {
            Status = ServiceResultStatus.Success;
            Message = string.Empty;
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public ServiceResultStatus Status { get; set; }

        [DataMember]
        public string Message { get; set; }

        #endregion Properties
    }

    [DataContract]
    public class ServiceResult<ResultType> : ServiceResult
    {
        #region Constructors

        /// <summary>
        /// Create a new generic ServiceResult
        /// </summary>
        public ServiceResult()
            : base()
        {
        }
        /// <summary>
        /// Create a new generic ServiceResult with a payload and successful status
        /// </summary>
        /// <param name="payload">Generic Payload</param>
        public ServiceResult(ResultType payload)
            : base()
        {
            Payload = payload;
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public ResultType Payload { get; set; }

        #endregion Properties
    }
}
