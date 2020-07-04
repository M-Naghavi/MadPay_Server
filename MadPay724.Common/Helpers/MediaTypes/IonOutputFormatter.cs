using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Common.Helpers.MediaTypes
{
    public class IonOutputFormatter : TextOutputFormatter
    {
        private readonly SystemTextJsonOutputFormatter _jsonOutputFormatter;
        public IonOutputFormatter(SystemTextJsonOutputFormatter jsonOutputFormatter)
        {
            if (jsonOutputFormatter == null)
                throw new ArgumentNullException(nameof(jsonOutputFormatter));
            else
                _jsonOutputFormatter = jsonOutputFormatter;

            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/ion+json"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
         => _jsonOutputFormatter.WriteResponseBodyAsync(context, selectedEncoding);

    }
}
