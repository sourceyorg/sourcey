using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.EntityTypeConfigurations;

namespace Sourcey.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyEventConfiguration(this ModelBuilder builder, string schema)
    {
        builder.ApplyConfiguration(new EventEntityTypeConfiguration(schema));

        return builder;
    }
}