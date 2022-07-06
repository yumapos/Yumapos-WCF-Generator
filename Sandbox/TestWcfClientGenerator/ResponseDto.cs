using System.Collections.Generic;
using System.Runtime.Serialization;
using YumaPos.Shared.API.ResponseDtos;

namespace TestDecoratorGeneration
{
    [DataContract]
    public class ResponseDto
    {
        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public CommandPostprocessingType? PostprocessingType { get; set; }

        public IEnumerable<ResponseErrorDto> Errors { get; set; }
        public object ServerInfo { get; set; }
        public object Value { get; set; }
        public IEnumerable<ResponseErrorDto> Warnings { get; set; }
    }

   

    public enum CommandPostprocessingType
    {
        LoginError,
        Ok
    }

    public interface ICommandExecutionResult
    {
        object Context { get; set; }

        CommandPostprocessingType? PostprocessingType { get; set; }

        object AdditionalInformation { get; set; }

        bool Success { get; set; }
    }

    public class CommandExecutionResult : ICommandExecutionResult
    {
        private object _context;
        private CommandPostprocessingType? _postprocessingType;
        private object _additionalInformation;
        private bool _success;

        #region Implementation of ICommandExecutionResult

        public object Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public CommandPostprocessingType? PostprocessingType
        {
            get { return _postprocessingType; }
            set { _postprocessingType = value; }
        }

        public object AdditionalInformation
        {
            get { return _additionalInformation; }
            set { _additionalInformation = value; }
        }

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        #endregion
    }
}