using YumaPos.FrontEnd.Infrastructure.CommandProcessing;

namespace YumaPos.Shared.API.ResponseDtos
{
    public class ResponseErrorDto
    {
        public CommandPostprocessingType? Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }
    }
}