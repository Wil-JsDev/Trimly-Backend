namespace Trimly.Presentation.ServiceExtension;

public static class Extension
{
    public static void UseSwaggerExtension(this IApplicationBuilder builder)
    {
        builder.UseSwagger();
        builder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json","Trimly");
        });
    }
}