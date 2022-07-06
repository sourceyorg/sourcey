using Zion.Core.Builder;

namespace Zion.AWS.SQS.Extensions
{
    public static class IZionBuilderExtensions
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
