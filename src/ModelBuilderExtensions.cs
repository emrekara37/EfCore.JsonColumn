using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EfCore.JsonColumn
{
    public static class ModelBuilderExtensions
    {

        public static Expression<Func<T, string>> FromExpression<T>()
        {
            // TODO : Read configuration
            return from => JsonSerializer.Serialize(from, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public static Expression<Func<string, T>> ToExpression<T>()
        {
            // TODO : Read configuration
            return to => JsonSerializer.Deserialize<T>(to,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
        public static ModelBuilder ApplyJsonColumns(this ModelBuilder modelBuilder)
        {

            var ignoreList = new List<Type>();
            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                var entityTypeBuilder = modelBuilder.Entity(mutableEntityType.ClrType);
                foreach (var property in mutableEntityType.ClrType.GetProperties())
                {
                    var attribute = property.GetCustomAttribute<ToJsonColumn>();
                    if (attribute != null)
                    {
                        var propertyBuilder = entityTypeBuilder.Property(property.Name);
                        // TODO :  Only works Sql Server
                        propertyBuilder
                            .HasColumnType("nvarchar(max)");
                        if (!string.IsNullOrEmpty(attribute.Name))
                        {
                            propertyBuilder.HasColumnName(attribute.Name);
                        }

                        var converterType = typeof(ValueConverter<,>);
                        Type[] typeArgs = { property.PropertyType, typeof(string) };

                        var makeGeneric = converterType.MakeGenericType(typeArgs);

                        MethodInfo fromExpressionMethodInfo = typeof(ModelBuilderExtensions).GetMethod(nameof(FromExpression));
                        MethodInfo toExpressionMethodInfo = typeof(ModelBuilderExtensions).GetMethod(nameof(ToExpression));
                        Type[] expressionTypeArgs = { property.PropertyType };
                        fromExpressionMethodInfo = fromExpressionMethodInfo?.MakeGenericMethod(expressionTypeArgs);
                        toExpressionMethodInfo = toExpressionMethodInfo?.MakeGenericMethod(expressionTypeArgs);

                        var fromExpression = fromExpressionMethodInfo?.Invoke(null, null);
                        var toExpression = toExpressionMethodInfo?.Invoke(null, null);

                        var converter = (ValueConverter)Activator.CreateInstance(makeGeneric, args: new object[]
                        {
                           fromExpression,
                           toExpression,
                           null
                        });

                        propertyBuilder.HasConversion(converter);

                        ignoreList.Add(property.PropertyType);
                    }
                }
            }

            foreach (var ignoredType in ignoreList)
            {
                modelBuilder.Ignore(ignoredType);
            }
            return modelBuilder;
        }
    }
}