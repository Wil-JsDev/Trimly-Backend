using Microsoft.Extensions.Logging;
using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Services;

public class ServicesService(
    IServiceRepository repository,
    ILogger<ServicesService> logger,
    ICloudinaryService cloudinaryService,
    IRegisteredCompanyRepository registeredCompanyRepository)
    : IServicesService
{
    public async Task<ResultT<PagedResult<ServicesDTos>>> GetPagedResult(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            logger.LogError("Invalid pagination parameters. PageNumber: {PageNumber}, PageSize: {PageSize}.",
                pageNumber, pageSize);

            return ResultT<PagedResult<ServicesDTos>>.Failure(Error.Failure("400",
                "Page number and page size must be greater than zero."));
        }

        var servicesNumber = await repository.GetPagedResultAsync(pageNumber, pageSize, cancellationToken);
        IEnumerable<ServicesDTos> servicesDTos = servicesNumber.Items.Select(x => new ServicesDTos
        (
            ServiceId: x.ServicesId,
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl,
            Status: x.Status,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: x.UpdateAt
        ));

        if (servicesDTos == null || !servicesDTos.Any())
        {
            logger.LogError(
                "No services found for the given pagination parameters. PageNumber: {PageNumber}, PageSize: {PageSize}.",
                pageNumber, pageSize);

            return ResultT<PagedResult<ServicesDTos>>.Failure(Error.Failure("400",
                "No services available for the requested page."));
        }

        PagedResult<ServicesDTos> pagedResult = new()
        {
            TotalItems = servicesNumber.TotalItems,
            CurrentPage = servicesNumber.CurrentPage,
            TotalPages = servicesNumber.TotalPages,
            Items = servicesDTos
        };

        logger.LogInformation("Successfully retrieved {TotalItems} services. Page {CurrentPage} of {TotalPages}.",
            servicesNumber.TotalItems, servicesNumber.CurrentPage, servicesNumber.TotalPages);

        return ResultT<PagedResult<ServicesDTos>>.Success(pagedResult);
    }

    public async Task<ResultT<ServicesDTos>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var service = await repository.GetByIdAsync(id, cancellationToken);
        if (service == null)
        {
            logger.LogError("{id} not found in services", id);

            return ResultT<ServicesDTos>.Failure(Error.Failure("404", $"{id} Service not found."));
        }

        ServicesDTos servicesDTos = new
        (
            ServiceId: service.ServicesId,
            Name: service.Name,
            Price: service.Price,
            Description: service.Description,
            DurationInMinutes: service.DurationInMinutes,
            ImageUrl: service.ImageUrl,
            Status: service.Status,
            RegisteredCompanyId: service.RegisteredCompanyId,
            CreatedAt: service.CreatedAt,
            UpdateAt: service.UpdateAt
        );

        logger.LogInformation("Successfully retrieved {ServiceId} service.", service.ServicesId);

        return ResultT<ServicesDTos>.Success(servicesDTos);
    }

    public async Task<ResultT<ServicesDTos>> CreateAsync(CreateServiceDTos createAppointmentDTos,
        CancellationToken cancellationToken)
    {
        if (createAppointmentDTos == null)
        {
            logger.LogError("Failed to create service. The request body is null.");

            return ResultT<ServicesDTos>.Failure(Error.Failure("400", "Invalid request. Service data is required."));
        }

        string? logoUrl = null;
        if (createAppointmentDTos.ImageFile != null)
        {
            using var stream = createAppointmentDTos.ImageFile.OpenReadStream();
            logoUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                createAppointmentDTos.ImageFile.FileName,
                cancellationToken);
        }

        Domain.Models.Services service = new()
        {
            ServicesId = Guid.NewGuid(),
            Name = createAppointmentDTos.Name,
            Price = createAppointmentDTos.Price,
            Description = createAppointmentDTos.Description,
            DurationInMinutes = createAppointmentDTos.DurationInMinutes,
            ImageUrl = logoUrl,
            Status = Status.Activated,
            RegisteredCompanyId = createAppointmentDTos.RegisteredCompanyId,
        };

        await repository.AddAsync(service, cancellationToken);

        ServicesDTos servicesDTos = new
        (
            ServiceId: service.ServicesId,
            Name: service.Name,
            Price: service.Price,
            Description: service.Description,
            DurationInMinutes: service.DurationInMinutes,
            ImageUrl: service.ImageUrl,
            Status: service.Status,
            RegisteredCompanyId: service.RegisteredCompanyId,
            CreatedAt: service.CreatedAt,
            UpdateAt: service.UpdateAt
        );
        logger.LogInformation("Service created successfully with ID: {ServiceId}.", service.ServicesId);

        return ResultT<ServicesDTos>.Success(servicesDTos);
    }

    public async Task<ResultT<ServicesDTos>> UpdateAsync(Guid id, UpdateServiceDTos entity,
        CancellationToken cancellation)
    {
        var service = await repository.GetByIdAsync(id, cancellation);
        if (service == null)
        {
            logger.LogError("Service update failed. Service with ID {ServiceId} not found.", id);

            return ResultT<ServicesDTos>.Failure(Error.NotFound("404", $"{id} Service not found."));
        }

        service.Name = entity.Name;
        service.Price = entity.Price;
        service.Description = entity.Description;
        service.DurationInMinutes = entity.DurationInMinutes;
        service.UpdateAt = DateTime.UtcNow;

        await repository.UpdateAsync(service, cancellation);
        ServicesDTos servicesDTos = new
        (
            ServiceId: service.ServicesId,
            Name: service.Name,
            Price: service.Price,
            Description: service.Description,
            DurationInMinutes: service.DurationInMinutes,
            ImageUrl: service.ImageUrl,
            Status: service.Status,
            RegisteredCompanyId: service.RegisteredCompanyId,
            CreatedAt: service.CreatedAt,
            UpdateAt: service.UpdateAt
        );

        logger.LogInformation("Service with ID {ServiceId} updated successfully.", id);

        return ResultT<ServicesDTos>.Success(servicesDTos);
    }

    public async Task<ResultT<Guid>> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        var service = await repository.GetByIdAsync(id, cancellation);
        if (service == null)
        {
            logger.LogError("Service with ID {ServiceId} not found.", id);

            return ResultT<Guid>.Failure(Error.NotFound("404", $"{id} Service not found."));
        }

        await repository.DeleteAsync(service, cancellation);

        logger.LogInformation("Service with ID {ServiceId} deleted successfully.", id);

        return ResultT<Guid>.Success(service.ServicesId ?? Guid.Empty);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesWithDurationLessThan30MinutesAsync(
        Guid registeredCompany, CancellationToken cancellationToken)
    {
        var company = await registeredCompanyRepository.GetByIdAsync(registeredCompany, cancellationToken);
        if (company == null)
        {
            logger.LogError("Company with ID {CompanyId} not found.", registeredCompany);

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.NotFound("404",
                $"Company with ID {registeredCompany} not found."));
        }

        var services =
            await repository.GetServicesWithDurationLessThan30MinutesAsync(registeredCompany, cancellationToken);
        if (!services.Any())
        {
            logger.LogWarning("No services found for company ID {CompanyId} with duration less than 30 minutes.",
                registeredCompany);

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400",
                "No services available with duration less than 30 minutes."));
        }

        IEnumerable<ServiceFilterDTos> serviceFilterDTos = services.Select(x => new ServiceFilterDTos
        (
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl
        ));

        logger.LogInformation(
            "Retrieved {ServiceCount} services with duration less than 30 minutes for company ID {CompanyId}.",
            serviceFilterDTos.Count(), registeredCompany);

        return ResultT<IEnumerable<ServiceFilterDTos>>.Success(serviceFilterDTos);
    }

    public async Task<ResultT<IEnumerable<ServicesDTos>>> GetServicesByCompanyIdAsync(Guid registeredCompaniesId,
        CancellationToken cancellationToken)
    {
        var company = await registeredCompanyRepository.GetByIdAsync(registeredCompaniesId, cancellationToken);
        if (company == null)
        {
            logger.LogError("Company with ID {CompanyId} not found.", registeredCompaniesId);

            return ResultT<IEnumerable<ServicesDTos>>.Failure(Error.NotFound("404",
                $"Company with ID {registeredCompaniesId} not found."));
        }

        var servicesByRegisteredCompanies =
            await repository.GetServicesByCompanyIdAsync(registeredCompaniesId, cancellationToken);

        if (servicesByRegisteredCompanies == null || !servicesByRegisteredCompanies.Any())
        {
            logger.LogError("No services found for company ID {CompanyId}.", registeredCompaniesId);

            return ResultT<IEnumerable<ServicesDTos>>.Failure(Error.Failure("400",
                "No services found for company ID."));
        }

        IEnumerable<ServicesDTos> servicesDTos = servicesByRegisteredCompanies.Select(x => new ServicesDTos
        (
            ServiceId: x.ServicesId,
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl,
            Status: x.Status,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: x.UpdateAt
        ));

        logger.LogInformation("Retrieved {Count} services for company ID {CompanyId}.", servicesDTos.Count(),
            registeredCompaniesId);

        return ResultT<IEnumerable<ServicesDTos>>.Success(servicesDTos);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByNameAsync(string name,
        Guid registeredCompanyId, CancellationToken cancellationToken)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            logger.LogError("Service search failed. The provided service name is null or empty.");

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(
                Error.Failure("400", "Service name cannot be empty."));
        }

        var exists = await repository.ValidateAsync(x => x.Name == name);
        if (!exists)
        {
            logger.LogError("Service search failed. No service found with the name '{ServiceName}'.", name);

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400",
                "No service found with the given name."));
        }

        var servicesByName = await repository.GetServicesByNameAsync(registeredCompanyId, name, cancellationToken);
        if (!servicesByName.Any())
        {
            logger.LogError("No services found for the company with ID {CompanyId} and service name '{ServiceName}'.",
                registeredCompanyId, name);

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400",
                "The service name is invalid."));
        }

        var servicesList = servicesByName.Select(x => new ServiceFilterDTos
        (
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl
        ));

        logger.LogInformation(
            "Successfully retrieved {ServiceCount} services for the company with ID {CompanyId} and service name '{ServiceName}'.",
            servicesList.Count(), registeredCompanyId, name);

        return ResultT<IEnumerable<ServiceFilterDTos>>.Success(servicesList);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByPriceAsync(decimal price,
        Guid registeredCompanyId, CancellationToken cancellationToken)
    {
        if (price <= 0)
        {
            logger.LogError("Service search failed. The provided price '{Price}' is invalid.", price);

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400",
                "Price must be greater than zero."));
        }

        var serviceByPrice = await repository.GetServicesByPriceAsync(registeredCompanyId, price, cancellationToken);
        IEnumerable<Domain.Models.Services> servicesEnumerable = serviceByPrice.ToList();
        if (!servicesEnumerable.Any())
        {
            logger.LogError("No services found for the company with ID {CompanyId} at price {Price}.",
                registeredCompanyId, price);

            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400",
                "No services found at the specified price."));
        }

        IEnumerable<ServiceFilterDTos> serviceFilterDTos = servicesEnumerable.Select(x => new ServiceFilterDTos
        (
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl
        ));
        IEnumerable<ServiceFilterDTos> serviceFilterDTosEnumerable = serviceFilterDTos.ToList();
        logger.LogInformation("Successfully retrieved {ServiceCount} services for the company with ID {CompanyId} at price {Price}.",
            serviceFilterDTosEnumerable.Count(), registeredCompanyId, price);

        return ResultT<IEnumerable<ServiceFilterDTos>>.Success(serviceFilterDTosEnumerable);
    }

    public async Task<ResultT<IEnumerable<ServicesDTos>>> GetServicesByDurationMinutesAsync(
        Guid registeredCompaniesId,
        int durationInMinutes,
        CancellationToken cancellationToken)
    {
        var registeredCompanies =
            await registeredCompanyRepository.GetByIdAsync(registeredCompaniesId, cancellationToken);
        if (registeredCompanies == null)
        {
            logger.LogError("Service search failed: No company found with ID {CompanyId}.", registeredCompaniesId);
            return ResultT<IEnumerable<ServicesDTos>>.Failure(Error.NotFound("404",$"No company found with ID '{registeredCompaniesId}'"));
        }

        var servicesByDurationInMinutes = await repository.GetServicesByDurationInMinutesAsync(
            registeredCompaniesId, 
            durationInMinutes, 
            cancellationToken);
        
        List<Domain.Models.Services> byDurationInMinutes = servicesByDurationInMinutes.ToList();

        if (!byDurationInMinutes.Any())
        {
            logger.LogWarning("No services found for company ID {CompanyId} with duration of {Duration} minutes.",
                registeredCompaniesId, durationInMinutes);
            return ResultT<IEnumerable<ServicesDTos>>.Failure(Error.NotFound("404",
                $"No services found with {durationInMinutes} minutes duration in the company."));
        }

        var servicesDTosEnumerable = byDurationInMinutes.Select(x => new ServicesDTos
        (
            ServiceId: x.ServicesId,
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl,
            Status: x.Status,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: x.UpdateAt
        ));
    
        logger.LogInformation("Successfully found {Count} services with {Duration} minutes duration for company ID {CompanyId}.",
            byDurationInMinutes.Count, durationInMinutes, registeredCompaniesId);

        return ResultT<IEnumerable<ServicesDTos>>.Success(servicesDTosEnumerable);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterMonthDTos>>> GetServicesByMonthAsync(Guid registeredCompanyId, int year,MonthFilter month,CancellationToken cancellationToken)
    {
        
        var registeredCompanies = await registeredCompanyRepository.GetByIdAsync(registeredCompanyId, cancellationToken);
        if (registeredCompanies == null)
        {
            logger.LogWarning("Company with ID '{RegisteredCompanyId}' was not found.", registeredCompanyId);
            
            return ResultT<IEnumerable<ServiceFilterMonthDTos>>.Failure(Error.NotFound("404", $"No company found with ID '{registeredCompanyId}'"));
        }
        
        var filterValue = FilterYearAndMonth(registeredCompanyId,year);

        if (filterValue.TryGetValue((month), out var result))
        {
            var filterMonthAndYear = await result(cancellationToken);
            IEnumerable<Domain.Models.Services> serviceFilterMonthDTos = filterMonthAndYear.ToList();
            
            if (!serviceFilterMonthDTos.Any())
            {
                logger.LogWarning("No completed services found for Company ID '{RegisteredCompanyId}' in {Month} {Year}.", registeredCompanyId, month, year);
                
                return ResultT<IEnumerable<ServiceFilterMonthDTos>>.Failure(Error.Failure("400", $"No completed services found for {month} {year}."));
            }

            IEnumerable<ServiceFilterMonthDTos> serviceFilterMonthDTosEnumerable = serviceFilterMonthDTos.Select(x => new ServiceFilterMonthDTos
            (
                NameService: x.Name,
                Price: x.Price,
                AboutService: x.Description,
                DurationInMinutes: x.DurationInMinutes,
                ServicesStatus: x.ServiceStatus
            ));

            var filterMonthDTosEnumerable = serviceFilterMonthDTosEnumerable.ToList();
            logger.LogInformation("Retrieved {Count} completed services for Company ID '{RegisteredCompanyId}' in {Month} {Year}.", filterMonthDTosEnumerable.Count(), registeredCompanyId, month, year);

            return ResultT<IEnumerable<ServiceFilterMonthDTos>>.Success(filterMonthDTosEnumerable);
        }
        
        logger.LogError("Invalid month filter: {Month} for year {Year}.", month, year);
        
        return ResultT<IEnumerable<ServiceFilterMonthDTos>>.Failure(Error.Failure("400", $"Invalid month filter: {month} for year {year}."));
    }
    
    #region Private Methods
    private Dictionary<MonthFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.Services>>>> FilterYearAndMonth(Guid registeredCompanyId, int year)
    {
        return Enum.GetValues<MonthFilter>().ToDictionary<MonthFilter, MonthFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.Services>>>>(
            month => month,
            month => new Func<CancellationToken, Task<IEnumerable<Domain.Models.Services>>> (async cancellationToken =>
            {
                int monthNumber = (int)month + 1; // Convert enum to number (January = 1, February = 2, etc.)
                return await repository.GetCompletedServicesByMonthAsync(registeredCompanyId, year, monthNumber, cancellationToken);
            })
        );
    }
    #endregion
    
}

