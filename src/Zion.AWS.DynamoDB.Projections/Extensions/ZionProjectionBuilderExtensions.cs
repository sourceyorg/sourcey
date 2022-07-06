using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Projections;
using Zion.Projections.Builder;

namespace Zion.AWS.DynamoDB.Projections.Extensions
{
    public static class ZionProjectionBuilderExtensions
    {
        public static IZionProjectionBuilder<TProjection> WithDynamoDBWriter<TProjection>(this IZionProjectionBuilder<TProjection> builder, Action<DynamoDBOptions> options)
            where TProjection : class, IProjection
        {
            builder.Services.RegisterDynamoDBWriter<TProjection>(options);
            return builder;
        }
    }
}
