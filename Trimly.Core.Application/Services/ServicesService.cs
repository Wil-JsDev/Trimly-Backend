using Microsoft.Extensions.Logging;
using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Services;

public class ServicesService : IServicesService
{
    private readonly IServiceRepository _repository;
    private readonly ILogger<ServicesService> _logger;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IRegisteredCompanyRepository _registeredCompanyRepository;
    
    public ServicesService(IServiceRepository repository, ILogger<ServicesService> logger, ICloudinaryService cloudinaryService, IRegisteredCompanyRepository registeredCompanyRepository)
    {
        _repository = repository;
        _logger = logger;
        _cloudinaryService = cloudinaryService;
        _registeredCompanyRepository = registeredCompanyRepository;
    }
    
    public async Task<ResultT<PagedResult<ServicesDTos>>> GetPagedResult(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogError("Invalid pagination parameters. PageNumber: {PageNumber}, PageSize: {PageSize}.", pageNumber, pageSize);
        
            return ResultT<PagedResult<ServicesDTos>>.Failure(Error.Failure("400", "Page number and page size must be greater than zero."));
        }
        
        var servicesNumber = await _repository.GetPagedResultAsync(pageNumber, pageSize, cancellationToken);
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

        if ( servicesDTos == null || !servicesDTos.Any())
        {
            _logger.LogError("No services found for the given pagination parameters. PageNumber: {PageNumber}, PageSize: {PageSize}.", 
                pageNumber, pageSize);
        
            return ResultT<PagedResult<ServicesDTos>>.Failure(Error.Failure("400", "No services available for the requested page."));
        }

        PagedResult<ServicesDTos> pagedResult = new()
        {
            TotalItems = servicesNumber.TotalItems,
            CurrentPage = servicesNumber.CurrentPage,
            TotalPages = servicesNumber.TotalPages,
            Items = servicesDTos
        };

        _logger.LogInformation("Successfully retrieved {TotalItems} services. Page {CurrentPage} of {TotalPages}.", 
            servicesNumber.TotalItems, servicesNumber.CurrentPage, servicesNumber.TotalPages);
        
        return ResultT<PagedResult<ServicesDTos>>.Success(pagedResult);
    }

    public async Task<ResultT<ServicesDTos>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var service = await _repository.GetByIdAsync(id, cancellationToken);
        if (service == null)
        {
            _logger.LogError("{id} not found in services",id);
            
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
        
        _logger.LogInformation("Successfully retrieved {ServiceId} service.", service.ServicesId);
        
        return ResultT<ServicesDTos>.Success(servicesDTos);
    }

    public async Task<ResultT<ServicesDTos>> CreateAsync(CreateServiceDTos createAppointmentDTos, CancellationToken cancellationToken)
    {
        if (createAppointmentDTos == null)
        {
            _logger.LogError("Failed to create service. The request body is null.");
        
            return ResultT<ServicesDTos>.Failure(Error.Failure("400", "Invalid request. Service data is required."));
        }
        
        string? logoUrl = null;
        if (createAppointmentDTos.ImageFile != null)
        {
            using var stream = createAppointmentDTos.ImageFile.OpenReadStream();
            logoUrl = await _cloudinaryService.UploadImageCloudinaryAsync(
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

        await _repository.AddAsync(service, cancellationToken);

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
        _logger.LogInformation("Service created successfully with ID: {ServiceId}.", service.ServicesId);
    
        return ResultT<ServicesDTos>.Success(servicesDTos);
    }

    public async Task<ResultT<ServicesDTos>> UpdateAsync(Guid id, UpdateServiceDTos entity, CancellationToken cancellation)
    {
        var service = await _repository.GetByIdAsync(id, cancellation);
        if (service == null)
        {
            _logger.LogError("Service update failed. Service with ID {ServiceId} not found.", id);
        
            return ResultT<ServicesDTos>.Failure(Error.NotFound("404", $"{id} Service not found."));
        }
        
        service.Name = entity.Name;
        service.Price = entity.Price;
        service.Description = entity.Description;
        service.DurationInMinutes = entity.DurationInMinutes;
        service.UpdateAt = DateTime.UtcNow;
        
        await _repository.UpdateAsync(service, cancellation);
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
        
        _logger.LogInformation("Service with ID {ServiceId} updated successfully.", id);
        
        return ResultT<ServicesDTos>.Success(servicesDTos);
    }

    public async Task<ResultT<Guid>> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        var service = await _repository.GetByIdAsync(id, cancellation);
        if (service == null)
        {
            _logger.LogError("");
            
            return ResultT<Guid>.Failure(Error.NotFound("404", $"{id} Service not found."));
        }
        
        await _repository.DeleteAsync(service, cancellation);
        
        _logger.LogInformation("Service with ID {ServiceId} deleted successfully.", id);
        
        return ResultT<Guid>.Success(service.ServicesId ?? Guid.Empty);
    }

    // string return
    public async Task<ResultT<Guid>> ApplyDiscountCodeAsync(Guid serviceId, Guid registeredCompanyId, string discountCode, CancellationToken cancellationToken)
    {

        var service = await _repository.GetByIdAsync(serviceId, cancellationToken);
        if (service == null)
        {
            _logger.LogError("Service with ID {ServiceId} not found.", serviceId);
            
            return ResultT<Guid>.Failure(Error.NotFound("404", $"{serviceId} Service not found."));
        }
        
        var company = await _registeredCompanyRepository.GetByIdAsync(registeredCompanyId, cancellationToken);
        if (company == null)
        {
            _logger.LogError("Company with ID {CompanyId} not found.", registeredCompanyId);
            
            return ResultT<Guid>.Failure(Error.NotFound("404", $"{registeredCompanyId} not found."));
        }

        if (string.IsNullOrWhiteSpace(discountCode))
        {
            _logger.LogError("Invalid discount code provided. Discount code cannot be null or empty.");
            
            return ResultT<Guid>.Failure(Error.Failure("400", $"The discount code {discountCode} is invalid."));
        } 
        
        await _repository.ApplyDiscountCodeAsync(service, registeredCompanyId, discountCode, cancellationToken);
        
        _logger.LogInformation("Successfully applied discount code {DiscountCode} to service ID {ServiceId} for company ID {CompanyId}.", 
            discountCode, serviceId, registeredCompanyId);
        
        return ResultT<Guid>.Success(service.ServicesId ?? Guid.Empty);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompany, CancellationToken cancellationToken)
    {
        var company = await _registeredCompanyRepository.GetByIdAsync(registeredCompany, cancellationToken);
        if (company == null)
        {
            _logger.LogError("Company with ID {CompanyId} not found.", registeredCompany);
        
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.NotFound("404", $"Company with ID {registeredCompany} not found."));
        }
        
        var services = await _repository.GetServicesWithDurationLessThan30MinutesAsync(registeredCompany, cancellationToken);
        if (!services.Any())
        {
            _logger.LogWarning("No services found for company ID {CompanyId} with duration less than 30 minutes.", registeredCompany);
        
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400", "No services available with duration less than 30 minutes."));
        }
        
        IEnumerable<ServiceFilterDTos> serviceFilterDTos = services.Select(x => new ServiceFilterDTos
        (
            Name: x.Name,
            Price:x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl
        ));
        
        _logger.LogInformation("Retrieved {ServiceCount} services with duration less than 30 minutes for company ID {CompanyId}.", 
            serviceFilterDTos.Count(), registeredCompany);
        
        return ResultT<IEnumerable<ServiceFilterDTos>>.Success(serviceFilterDTos);
    }

    public async Task<ResultT<IEnumerable<ServicesDTos>>> GetServicesByCompanyIdAsync(Guid registeredCompaniesId, CancellationToken cancellationToken)
    {
        var company = await _registeredCompanyRepository.GetByIdAsync(registeredCompaniesId, cancellationToken);
        if (company == null)
        {
            _logger.LogError("Company with ID {CompanyId} not found.", registeredCompaniesId);
            
            return ResultT<IEnumerable<ServicesDTos>>.Failure(Error.NotFound("404", $"Company with ID {registeredCompaniesId} not found."));
        }

        var servicesByRegisteredCompanies =
            await _repository.GetServicesByCompanyIdAsync(registeredCompaniesId, cancellationToken);
        
        if (servicesByRegisteredCompanies == null || !servicesByRegisteredCompanies.Any())
        {
            _logger.LogError("No services found for company ID {CompanyId}.", registeredCompaniesId);
            
            return ResultT<IEnumerable<ServicesDTos>>.Failure(Error.Failure("400", "No services found for company ID."));
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
        
        _logger.LogInformation("Retrieved {Count} services for company ID {CompanyId}.", servicesDTos.Count(), registeredCompaniesId);

        return ResultT<IEnumerable<ServicesDTos>>.Success(servicesDTos);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByNameAsync(string name, Guid registeredCompanyId, CancellationToken cancellationToken)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            _logger.LogError("Service search failed. The provided service name is null or empty.");
            
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400", "Service name cannot be empty."));
        }
        
        var exists = await _repository.ValidateAsync(x => x.Name == name);
        if (!exists)
        {
            _logger.LogError("Service search failed. No service found with the name '{ServiceName}'.", name);
        
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400", "No service found with the given name."));
        }

        var servicesByName = await _repository.GetServicesByNameAsync(registeredCompanyId,name, cancellationToken);
        if (!servicesByName.Any())
        {
            _logger.LogError("No services found for the company with ID {CompanyId} and service name '{ServiceName}'.", registeredCompanyId, name);
        
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400", "The service name is invalid."));
        }

        var servicesList = servicesByName.Select(x => new ServiceFilterDTos
        (
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl
        ));
        
        _logger.LogInformation("Successfully retrieved {ServiceCount} services for the company with ID {CompanyId} and service name '{ServiceName}'.", 
            servicesList.Count(), registeredCompanyId, name);

        return ResultT<IEnumerable<ServiceFilterDTos>>.Success(servicesList);
    }

    public async Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByPriceAsync(decimal price, Guid registeredCompanyId, CancellationToken cancellationToken)
    {
        if (price <= 0)
        {
            _logger.LogError("Service search failed. The provided price '{Price}' is invalid.", price);
        
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400", "Price must be greater than zero."));
        }
        
        var serviceByPrice = await _repository.GetServicesByPriceAsync(registeredCompanyId, price, cancellationToken);
        if (!serviceByPrice.Any())
        {
            _logger.LogError("No services found for the company with ID {CompanyId} at price {Price}.", registeredCompanyId, price);
        
            return ResultT<IEnumerable<ServiceFilterDTos>>.Failure(Error.Failure("400", "No services found at the specified price."));
        }
        IEnumerable<ServiceFilterDTos> serviceFilterDTos = serviceByPrice.Select(x => new ServiceFilterDTos
        (
            Name: x.Name,
            Price: x.Price,
            Description: x.Description,
            DurationInMinutes: x.DurationInMinutes,
            ImageUrl: x.ImageUrl
        ));
        _logger.LogInformation("Successfully retrieved {ServiceCount} services for the company with ID {CompanyId} at price {Price}.", 
            serviceFilterDTos.Count(), registeredCompanyId, price);

        return ResultT<IEnumerable<ServiceFilterDTos>>.Success(serviceFilterDTos);
    }
}