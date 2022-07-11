using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDecoratorGeneration;

namespace YumaPos.Shared.API.ResponseDtos
{
    public class ResponseErrorDto
    {
        public CommandPostprocessingType? Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }
    }
}
