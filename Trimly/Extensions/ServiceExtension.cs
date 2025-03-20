using Asp.Versioning;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Application.DTOs.Review;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.DTOs.Service;
using Trimly.Presentation.ExceptionHandling;
using Trimly.Presentation.Validations.Appointment;
using Trimly.Presentation.Validations.RegisteredCompanies;
using Trimly.Presentation.Validations.Review;
using Trimly.Presentation.Validations.Schedules;
using Trimly.Presentation.Validations.Services;

namespace Trimly.Presentation.Extensions;

public static class ServiceExtension
{
    public static void AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Trimly",
                Description =
                    "Barbershop Appointment Bookings",
                Contact = new OpenApiContact
                {
                    Name = "Wilmer De La Cruz y Dayron Bello",
                    Email = "trimlydw@gmail.com"
                }
            });
        });
    }

    public static void AddValidation(this IServiceCollection services )
    {
        services.AddScoped<IValidator<CreateRegisteredCompaniesDTos>,CreateRegisteredCompanies>();
        services.AddScoped<IValidator<RegisteredCompaniesUpdateDTos>,UpdateRegisteredCompanies>();
        services.AddScoped<IValidator<CreateServiceDTos>, CreateService>();
        services.AddScoped<IValidator<UpdateServiceDTos>, UpdateService>();
        services.AddScoped<IValidator<CreateAppointmentDTos>, CreateAppointment>();
        services.AddScoped<IValidator<UpdateAppoinmentDTos>, UpdateAppointment>();
        services.AddScoped<IValidator<CreateReviewsDTos>, CreateReview>();
        services.AddScoped<IValidator<ReviewsUpdateDTos>, UpdateReview>();
        services.AddScoped<IValidator<CreateSchedulesDTos>, CreateSchedules>();
        services.AddScoped<IValidator<UpdateSchedulesDTos>, UpdateSchedules>();
    }
    
    public static void AddException(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddExceptionHandler<DbUpdateExceptionHandler>();
        services.AddProblemDetails();
    }
    
    public static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
    }
}