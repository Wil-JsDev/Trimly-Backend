using Microsoft.Extensions.Logging;
using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Services;

public class RegisteredCompaniesService : IRegisteredCompaniesService
{
    private readonly IRegisteredCompanyRepository _registeredCompanyRepository;
    private readonly ILogger<RegisteredCompaniesService> _logger;
    private readonly ICloudinaryService _cloudinaryService;
    public RegisteredCompaniesService(IRegisteredCompanyRepository registeredCompanyRepository, ILogger<RegisteredCompaniesService> logger, ICloudinaryService cloudinaryService) 
    {
        _registeredCompanyRepository = registeredCompanyRepository;
        _logger = logger;
        _cloudinaryService = cloudinaryService;
    }
    
    public async Task<ResultT<PagedResult<RegisteredCompaniesDTos>>> GetPagedResult(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0 && pageSize <= 0)
        {
            _logger.LogError("Invalid pagination parameters: pageNumber and pageSize must be greater than 1.");
    
            return ResultT<PagedResult<RegisteredCompaniesDTos>>.Failure(
                Error.Failure("400", "Invalid pagination parameters. Both pageNumber and pageSize must be greater than 1.")
            );
        }
        var registeredCompanyWithNumber = await _registeredCompanyRepository.GetPagedResultAsync(pageNumber, pageSize, cancellationToken);
        var dtoItems = registeredCompanyWithNumber.Items.Select(x => new RegisteredCompaniesDTos
        (
            RegisteredCompaniesId: x.RegisteredCompaniesId,
            Name: x.Name,
            PhoneNumber: x.Phone,
            AddresCompanies: x.Address,
            DescriptionCompanies: x.Description,
            LogoUrl: x.LogoUrl,
            Status: x.Status,
            RegistrationDateCompany: x.RegistrationDate
        )).ToList();

        if (!dtoItems.Any())
        {
            _logger.LogError("No register companies found");

            return ResultT<PagedResult<RegisteredCompaniesDTos>>.Failure(Error.Failure("404", "The list is empty"));
        }

        PagedResult<RegisteredCompaniesDTos> pagedResult = new()
        {
            TotalItems = registeredCompanyWithNumber.TotalItems,
            CurrentPage = registeredCompanyWithNumber.CurrentPage,
            TotalPages = registeredCompanyWithNumber.TotalPages,
            Items = dtoItems
        };
        
        _logger.LogInformation("Successfully retrieved {Count} registered companies. Page {CurrentPage} of {TotalPages}.", 
            dtoItems.Count, pagedResult.CurrentPage, pagedResult.TotalPages);
        
        return ResultT<PagedResult<RegisteredCompaniesDTos>>.Success(pagedResult);
    }

    public async Task<ResultT<RegisteredCompaniesDTos>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var registeredCompany = await _registeredCompanyRepository.GetByIdAsync(id, cancellationToken);
        if (registeredCompany == null)
        {
            _logger.LogError("No register companies found");

            return ResultT<RegisteredCompaniesDTos>.Failure(Error.Failure("404", "The register companies could not be found"));
        }

        RegisteredCompaniesDTos registeredCompaniesDTos = new
        (
            RegisteredCompaniesId: registeredCompany.RegisteredCompaniesId,
            Name: registeredCompany.Name,
            PhoneNumber: registeredCompany.Phone,
            AddresCompanies: registeredCompany.Address,
            DescriptionCompanies: registeredCompany.Description,
            LogoUrl: registeredCompany.LogoUrl,
            Status: registeredCompany.Status,
            RegistrationDateCompany: registeredCompany.RegistrationDate
        );
        
        _logger.LogInformation("Register companies found");

        return ResultT<RegisteredCompaniesDTos>.Success(registeredCompaniesDTos);
    }

    public async Task<ResultT<RegisteredCompaniesDTos>> CreateAsync(CreateRegisteredCompaniesDTos createAppointmentDTos, CancellationToken cancellationToken)
    {
        if (createAppointmentDTos == null)
        {
            _logger.LogError("Invalid parameter: createRegisteredCompaniesDTos");
            
            return ResultT<RegisteredCompaniesDTos>.Failure(Error.Failure("400", "Invalid parameter: createRegisteredCompaniesDTos"));
        }

        var exists = await _registeredCompanyRepository.ValidateAsync(x => x.Name == createAppointmentDTos.Name);
        if (exists)
        {
            _logger.LogError("Company registration failed: A company with the name '{CompanyName}' already exists.", createAppointmentDTos.Name);
    
            return ResultT<RegisteredCompaniesDTos>.Failure(
                Error.Failure("400", $"A company with the name '{createAppointmentDTos.Name}' already exists. Please choose a different name.")
            );
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
        
        Domain.Models.RegisteredCompanies registeredCompany = new()
        {
            RegisteredCompaniesId = Guid.NewGuid(),
            Name = createAppointmentDTos.Name,
            RNC = createAppointmentDTos.Rnc,
            Phone = createAppointmentDTos.PhoneNumber,
            Address = createAppointmentDTos.AddresCompanies,
            Email = createAppointmentDTos.Email,
            Description = createAppointmentDTos.DescriptionCompanies,
            LogoUrl = logoUrl,
            Status = Domain.Enum.Status.Activated,
        };
        
        await _registeredCompanyRepository.AddAsync(registeredCompany, cancellationToken);
        
        RegisteredCompaniesDTos companiesDTos = new
        (
            RegisteredCompaniesId: registeredCompany.RegisteredCompaniesId,
            Name: registeredCompany.Name,
            PhoneNumber: registeredCompany.Phone,
            AddresCompanies: registeredCompany.Address,
            DescriptionCompanies: registeredCompany.Description,
            LogoUrl: logoUrl,
            Status: registeredCompany.Status,
            RegistrationDateCompany: registeredCompany.RegistrationDate
        );
        
        _logger.LogInformation("Company '{CompanyName}' has been successfully registered with ID {CompanyId}.", 
            companiesDTos.Name, companiesDTos.RegisteredCompaniesId);
        
        return ResultT<RegisteredCompaniesDTos>.Success(companiesDTos);
    }

    public async Task<ResultT<RegisteredCompaniesDTos>> UpdateAsync(Guid id, RegisteredCompaniesUpdateDTos entity, CancellationToken cancellation)
    {
        var registeredCompany = await _registeredCompanyRepository.GetByIdAsync(id, cancellation);
        if (registeredCompany == null)
        {
            _logger.LogError("No register companies found");

            return ResultT<RegisteredCompaniesDTos>.Failure(Error.Failure("404", $"{id} is not registered"));
        }

        var existsName = await _registeredCompanyRepository.ValidateAsync(x => x.Name == entity.Name);
        if (existsName)
        {
            _logger.LogError("Company registration failed: Name already exists.");
            
            return ResultT<RegisteredCompaniesDTos>.Failure(Error.Failure("400", $"{id} is already registered"));
        }
        
        string? logoUrl = null;
        if (entity.ImageFile != null)
        {
            using var stream = entity.ImageFile.OpenReadStream();
            logoUrl = await _cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                entity.ImageFile.FileName,
                cancellation);
        }
        
        registeredCompany.Name = entity.Name;
        registeredCompany.Phone = entity.Phone;
        registeredCompany.Email = entity.Email;
        registeredCompany.Description = entity.DescriptionCompanies;
        registeredCompany.LogoUrl = logoUrl;
        registeredCompany.Address = entity.AddressCompany;
        registeredCompany.Status = entity.Status;
        
        await _registeredCompanyRepository.UpdateAsync(registeredCompany, cancellation);

        RegisteredCompaniesDTos companiesDTos = new
        (
            RegisteredCompaniesId: registeredCompany.RegisteredCompaniesId,
            Name: registeredCompany.Name,
            PhoneNumber: registeredCompany.Phone,
            AddresCompanies: registeredCompany.Address,
            DescriptionCompanies: registeredCompany.Description,
            LogoUrl: registeredCompany.LogoUrl,
            Status: registeredCompany.Status,
            RegistrationDateCompany: registeredCompany.RegistrationDate
        );
        
        _logger.LogInformation("Company '{CompanyName}' (ID: {CompanyId}) has been successfully updated.", 
            registeredCompany.Name, registeredCompany.RegisteredCompaniesId);
        
        return ResultT<RegisteredCompaniesDTos>.Success(companiesDTos);
    }

    public async Task<ResultT<Guid>> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        var registeredCompany = await _registeredCompanyRepository.GetByIdAsync(id, cancellation);
        if (registeredCompany == null)
        {
            _logger.LogError("No register companies found");

            return ResultT<Guid>.Failure(Error.Failure("404", $"{id} is not registered"));
        }
        
        await _registeredCompanyRepository.DeleteAsync(registeredCompany, cancellation);

        _logger.LogInformation("Company with ID {CompanyId} has been successfully deleted.", id);
        
        return ResultT<Guid>.Success(registeredCompany.RegisteredCompaniesId ?? Guid.Empty);
    }

    public async Task<ResultT<IEnumerable<OrderNameComapanyDTos>>> FilterByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogError("Validation failed: The 'name' parameter is missing or empty.");
    
            return ResultT<IEnumerable<OrderNameComapanyDTos>>.Failure(
                Error.Failure("400", "The 'name' field is required and cannot be empty.")
            );
        }

        var exists = await _registeredCompanyRepository.ValidateAsync(x => x.Name == name);
        if (!exists)
        {
            _logger.LogError("Validation failed: The company '{name}' is not registered.",name);
    
            return ResultT<IEnumerable<OrderNameComapanyDTos>>.Failure(
                Error.Failure("404", $"The company '{name}' is not registered.")
            );
        }

        var filterByName = await _registeredCompanyRepository.OrderByNameAsync(cancellationToken);
        if (filterByName == null || !filterByName.Any())
        {
            _logger.LogWarning("No registered companies found."); 

            return ResultT<IEnumerable<OrderNameComapanyDTos>>.Failure(
                Error.Failure("404", "No registered companies found.")
            );
        }
        
        IEnumerable<OrderNameComapanyDTos> comapanyDTos =
            filterByName.Select(x => new OrderNameComapanyDTos
        (
            NameRegistedCompanyDTos: x.Name
        ));
        
        _logger.LogInformation("Successfully retrieved {Count} registered companies matching the name '{CompanyName}'.", 
            comapanyDTos.Count(), name);
        
        return ResultT<IEnumerable<OrderNameComapanyDTos>>.Success(comapanyDTos);
    }

    public async Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> FilterByStatusAsync(Status status, CancellationToken cancellationToken)
    {
        var exists = await _registeredCompanyRepository.ValidateAsync(x => x.Status == status);
        if (!exists)
        {
            _logger.LogError("No registered companies found with status: {Status}", status);

            return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Failure(
                Error.NotFound("404", $"No companies found with status '{status}'.")
            );
        }

        var filterByStatus = await _registeredCompanyRepository.FilterByStatusAsync(status, cancellationToken);
        if (filterByStatus == null || !filterByStatus.Any())
        {
            _logger.LogWarning("Filter result is empty for status: {Status}", status);

            return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Failure(
                Error.NotFound("404", $"No companies found with status '{status}'.")
            );
        }

        IEnumerable<RegisteredCompaniesDTos> comapanyDTos = filterByStatus.Select(x => new RegisteredCompaniesDTos
        (
            RegisteredCompaniesId: x.RegisteredCompaniesId,
            Name: x.Name,
            PhoneNumber: x.Phone,
            AddresCompanies: x.Address,
            DescriptionCompanies: x.Description,
            LogoUrl: x.LogoUrl,
            Status: x.Status,
            RegistrationDateCompany: x.RegistrationDate
        ));
        
        _logger.LogInformation("Successfully retrieved {Count} registered companies.", comapanyDTos.Count());

        return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Success(comapanyDTos);
    }


    public async Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> OrderByNameAsync(CancellationToken cancellationToken)
    {
        var registeredCompaniesOrderNames = await _registeredCompanyRepository.OrderByNameAsync(cancellationToken);
        if (!registeredCompaniesOrderNames.Any())
        {
            _logger.LogWarning("No registered companies found.");

            return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Failure(Error.NotFound("404",
                $"No registered companies found."));
        }

        IEnumerable<RegisteredCompaniesDTos> companiesDTos = registeredCompaniesOrderNames.Select(x => new RegisteredCompaniesDTos
        (
            RegisteredCompaniesId: x.RegisteredCompaniesId,
            Name: x.Name,
            PhoneNumber: x.Phone,
            AddresCompanies: x.Address,
            DescriptionCompanies: x.Description,
            LogoUrl: x.LogoUrl,
            Status: x.Status,
            RegistrationDateCompany: x.RegistrationDate
        ));

        _logger.LogInformation("Successfully retrieved {Count} registered companies ordered by name.", companiesDTos.Count());
        
        return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Success(companiesDTos);
    }
    
    public async Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> GetRecentAsync(CancellationToken cancellationToken)
    {
        var registeredCompaniesRecent = await _registeredCompanyRepository.GetRecentAsync(cancellationToken);
        if (  registeredCompaniesRecent == null || !registeredCompaniesRecent.Any())
        {
            _logger.LogError("No recent registered companies found.");

            return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Failure(Error.NotFound("404", "The list is empty"));
        }

        IEnumerable<RegisteredCompaniesDTos> companiesDTos = registeredCompaniesRecent.Select(x => new RegisteredCompaniesDTos
        (
            RegisteredCompaniesId: x.RegisteredCompaniesId,
            Name: x.Name,
            PhoneNumber: x.Phone,
            AddresCompanies: x.Address,
            DescriptionCompanies: x.Description,
            LogoUrl: x.LogoUrl,
            Status: x.Status,
            RegistrationDateCompany: x.RegistrationDate
        ));
        
        _logger.LogInformation("Successfully retrieved {Count} recent registered companies.", companiesDTos.Count());
        
        return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Success(companiesDTos);
    }

    public async Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> OrderByIdAsync(string order, CancellationToken cancellationToken)
    {
        var companyKey = GetSort();
        if (companyKey.TryGetValue((order), out var companyOrder))
        {
            var orderId = await companyOrder(cancellationToken);
            if (orderId == null || !orderId.Any())
            {
                _logger.LogError("Attempted to fetch registered companies, but the provided order ID list was null or empty.");
                
                return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Failure(Error.NotFound("404", "The list is empty"));
            }

            IEnumerable<RegisteredCompaniesDTos> companiesDTos = orderId.Select(x => new RegisteredCompaniesDTos
            (
                RegisteredCompaniesId: x.RegisteredCompaniesId,
                Name: x.Name,
                PhoneNumber: x.Phone,
                AddresCompanies: x.Address,
                DescriptionCompanies: x.Description,
                LogoUrl: x.LogoUrl,
                Status: x.Status,
                RegistrationDateCompany: x.RegistrationDate
            ));
            
            _logger.LogInformation("Registered companies data fetched successfully. Returning {Count} results.", companiesDTos.Count());
            
            return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Success(companiesDTos);
        }
        _logger.LogError("Invalid order key '{Order}'. No matching sorting function found.", order);
        
        return ResultT<IEnumerable<RegisteredCompaniesDTos>>.Failure(Error.NotFound("404", "No registered companies found."));
    }
    
    #region Private methods
    private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.RegisteredCompanies>>>> GetSort()
    {
        return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<RegisteredCompanies>>>>()
        {
            {"asc", async cancellationToken => await _registeredCompanyRepository.OrderByIdAscAsync(cancellationToken)},
            {"desc", async cancellationToken => await _registeredCompanyRepository.OrderByIdDescAsync(cancellationToken)}
        };
    }
    #endregion
}