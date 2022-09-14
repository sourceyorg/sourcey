using Zion.AWS.SQS;
using Zion.Core.Builder;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddAWSSQS(this IZionBuilder builder, Action<SQSOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddAWSSQS(optionsAction);

            return builder;
        }
    }
}
