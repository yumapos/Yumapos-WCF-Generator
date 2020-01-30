using System;

namespace YumaPos.Shared.API.ResponseDtos
{
    public class ResponseServerInfoDto
    {
        public DateTime UtcTime { get; set; }

        public string Version { get; set; }
    }
}