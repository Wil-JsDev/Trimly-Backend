using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Trimly.Core.Application.DTOs.Account;
using Trimly.Core.Application.DTOs.Account.Authenticate;
using Trimly.Core.Application.DTOs.Account.Password;
using Trimly.Core.Application.DTOs.Account.Register;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Application.DTOs.Review;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.DTOs.Service;
using Trimly.Presentation.ExceptionHandling;
using Trimly.Presentation.Validations.Account;
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
        services.AddScoped<IValidator<AuthenticateRequest>, AuthenticateAccount>();
        services.AddScoped<IValidator<ForgotRequest>, ForgotPasswordAccount>();
        services.AddScoped<IValidator<RegisterRequest>, RegisterAccount>();
        services.AddScoped<IValidator<ResetPasswordRequest>,ResetPasswordAccount>();
        services.AddScoped<IValidator<UpdateAccountDto>, UpdateAccount>();
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

    public static void AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode= StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, toke) =>
            {
                await context.HttpContext.Response.WriteAsync("Request limit exceeded. Please try again later", cancellationToken: toke);
            };

            options.AddFixedWindowLimiter("fixed", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromSeconds(10);
                limiterOptions.PermitLimit = 5;
            });

            options.AddSlidingWindowLimiter("sliding", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.SegmentsPerWindow = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(4);
            });

            options.AddTokenBucketLimiter("tokenBucketPolicy", limiterOptions =>
            {
                limiterOptions.TokenLimit = 10; // 10 tokens maximum
                limiterOptions.TokensPerPeriod = 2;         // 2 tokens are recharged
                limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(5); // Every 5 seconds
            });
        });
    }
    
}