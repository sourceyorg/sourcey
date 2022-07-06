using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.Core.Keys;
using Zion.EntityFrameworkCore.ChangeTracking;
using Zion.EntityFrameworkCore.ValueConversion;

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> builder)
            where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new JsonValueConverter<T>())
                   .Metadata
                   .SetValueComparer(new JsonValueComparer<T>());

            return builder;
        }

        public static PropertyBuilder<Causation> HasCausationValueConversion(this PropertyBuilder<Causation> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new CausationValueConverter())
                   .Metadata
                   .SetValueComparer(new CausationValueComparer());

            return builder;
        }

        public static PropertyBuilder<Causation?> HasNullableCausationValueConversion(this PropertyBuilder<Causation?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableCausationValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableCausationValueComparer());

            return builder;
        }

        public static PropertyBuilder<Correlation> HasCorrelationValueConversion(this PropertyBuilder<Correlation> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new CorrelationValueConverter())
                   .Metadata
                   .SetValueComparer(new CorrelationValueComparer());

            return builder;
        }

        public static PropertyBuilder<Correlation?> HasNullableCorrelationValueConversion(this PropertyBuilder<Correlation?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableCorrelationValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableCorrelationValueComparer());

            return builder;
        }

        public static PropertyBuilder<Actor> HasActorValueConversion(this PropertyBuilder<Actor> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new ActorValueConverter())
                   .Metadata
                   .SetValueComparer(new ActorValueComparer());

            return builder;
        }

        public static PropertyBuilder<Actor?> HasNullableActorValueConversion(this PropertyBuilder<Actor?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableActorValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableActorValueComparer());

            return builder;
        }

        public static PropertyBuilder<Subject> HasSubjectValueConversion(this PropertyBuilder<Subject> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new SubjectValueConverter())
                   .Metadata
                   .SetValueComparer(new SubjectValueComparer());

            return builder;
        }

        public static PropertyBuilder<Subject?> HasNullableSubjectValueConversion(this PropertyBuilder<Subject?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableSubjectValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableSubjectValueComparer());

            return builder;
        }
    }
}
