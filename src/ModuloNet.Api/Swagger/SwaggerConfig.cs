using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModuloNet.Api.Swagger;

public static class SwaggerConfig
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ModuloNet API",
            Version = "v1",
            Description = "Modular Monolith + Vertical Slice template"
        });
    }
}
