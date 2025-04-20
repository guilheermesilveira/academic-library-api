using Library.Api.Configurations.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Library.Api.Configurations;

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.UseDateOnlyTimeOnlyStringConverters();

            options.OperationFilter<FileUploadFilter>();
            options.OperationFilter<SwaggerDefaultValues>();
            options.DocumentFilter<LowercaseDocumentFilter>();

            options.OrderActionsBy(apiDescription => apiDescription.GroupName);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert the JWT token like this: Bearer {your token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void UseSwaggerConfig(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var groupName in provider.ApiVersionDescriptions.Select(description => description.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
            }
        });
    }
}