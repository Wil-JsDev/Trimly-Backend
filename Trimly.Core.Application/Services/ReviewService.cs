using Microsoft.Extensions.Logging;
using Trimly.Core.Application.DTOs.Review;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly ILogger<ReviewService> _logger;
    private readonly IRegisteredCompanyRepository _registeredCompanyRepository;
    
    public ReviewService(IReviewsRepository reviewsRepository, ILogger<ReviewService> logger, IRegisteredCompanyRepository registeredCompanyRepository)
    {
        _reviewsRepository = reviewsRepository;
        _logger = logger;
        _registeredCompanyRepository = registeredCompanyRepository;
    }
    
    public async Task<ResultT<PagedResult<ReviewsDTos>>> GetPagedResult(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0 && pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters: pageNumber and pageSize must be greater than zero.");
        
            return ResultT<PagedResult<ReviewsDTos>>.Failure(Error.Failure("400", "Both pageNumber and pageSize must be greater than zero."));
        }
        
        var reviewNumber = await _reviewsRepository.GetPagedResultAsync(pageNumber, pageSize, cancellationToken);
        IEnumerable<ReviewsDTos> reviewsPagedItems = reviewNumber.Items.Select(x => new ReviewsDTos
        (
            ReviewId: x.ReviewId,
            Title: x.Title,
            Comment: x.Comment,
            Rating: x.Rating,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: x.UpdateAt
        ));

        PagedResult<ReviewsDTos> pagedResult = new PagedResult<ReviewsDTos>()
        {
            TotalItems = reviewNumber.TotalItems,
            Items = reviewsPagedItems,
            TotalPages = reviewNumber.TotalPages,
            CurrentPage = reviewNumber.CurrentPage
        };
        
        _logger.LogInformation("Successfully retrieved paginated reviews: Page {CurrentPage} of {TotalPages}, TotalItems: {TotalItems}", 
            pagedResult.CurrentPage, pagedResult.TotalPages, pagedResult.TotalItems);

        return ResultT<PagedResult<ReviewsDTos>>.Success(pagedResult);
    }

    public async Task<ResultT<ReviewsDTos>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetByIdAsync(id, cancellationToken);
        if (review == null)
        {
            _logger.LogWarning("Review with ID {ReviewId} was not found.", id);
        
            return ResultT<ReviewsDTos>.Failure(Error.NotFound("404", $"Review with ID {id} was not found."));
        }
        
        ReviewsDTos reviewsDTos = new
        (
            ReviewId: review.ReviewId,
            Title: review.Title,
            Comment: review.Comment,
            Rating: review.Rating,
            RegisteredCompanyId: review.RegisteredCompanyId,
            CreatedAt: review.CreatedAt,
            UpdateAt: review.UpdateAt
        );
        
        _logger.LogInformation("Review with ID {ReviewId} was successfully retrieved.", id);

        return ResultT<ReviewsDTos>.Success(reviewsDTos);
    }

    public async Task<ResultT<ReviewsDTos>> CreateAsync(CreateReviewsDTos createAppointmentDTos, CancellationToken cancellationToken)
    {
        if (createAppointmentDTos == null)
        {
            _logger.LogWarning("The request to create a review is null.");
            
            return ResultT<ReviewsDTos>.Failure(Error.Failure("400", "Review data is required."));
        }

        Domain.Models.Reviews reviews = new()
        {
            ReviewId = Guid.NewGuid(),
            Title = createAppointmentDTos.Title,
            Comment = createAppointmentDTos.Comment,
            Rating = createAppointmentDTos.Rating,
            RegisteredCompanyId = createAppointmentDTos.RegisteredCompanyId
        };
        
        await _reviewsRepository.AddAsync(reviews, cancellationToken);
        
        _logger.LogInformation("Review with ID {ReviewId} has been successfully created.", reviews.ReviewId);

        ReviewsDTos reviewsDTos = new
        (
            ReviewId: reviews.ReviewId,
            Title: reviews.Title,
            Comment: reviews.Comment,
            Rating: reviews.Rating,
            RegisteredCompanyId: reviews.RegisteredCompanyId,
            CreatedAt: reviews.CreatedAt,
            UpdateAt: reviews.UpdateAt
        );
        
        return ResultT<ReviewsDTos>.Success(reviewsDTos);
    }

    public async Task<ResultT<ReviewsDTos>> UpdateAsync(Guid id, ReviewsUpdateDTos entity, CancellationToken cancellation)
    {
        var review = await _reviewsRepository.GetByIdAsync(id, cancellation);
        if (review == null)
        {
            _logger.LogError("Review with ID {ReviewId} was not found for update.", id);
            
            return ResultT<ReviewsDTos>.Failure(Error.NotFound("404", $"Review with ID {id} was not found."));
        }
        
        review.Title = entity.Title;
        review.Comment = entity.Comment;
        review.Rating = entity.Rating;
        
        await _reviewsRepository.UpdateAsync(review, cancellation);
        
        _logger.LogInformation("Review with ID {ReviewId} has been successfully updated.", id);

        ReviewsDTos reviewsDTos = new
        (
            ReviewId: review.ReviewId,
            Title: review.Title,
            Comment: review.Comment,
            Rating: review.Rating,
            RegisteredCompanyId: review.RegisteredCompanyId,
            CreatedAt: review.CreatedAt,
            UpdateAt: review.UpdateAt
        );

        return ResultT<ReviewsDTos>.Success(reviewsDTos);
    }

    public async Task<ResultT<Guid>> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        var review = await _reviewsRepository.GetByIdAsync(id, cancellation);
        if (review == null)
        {
            _logger.LogError("Review with ID {ReviewId} was not found for deletion.", id);
            
            return ResultT<Guid>.Failure(Error.NotFound("404", $"Review with ID {id} was not found."));
        }
        
        await _reviewsRepository.DeleteAsync(review, cancellation);
        
        _logger.LogInformation("Review with ID {ReviewId} has been successfully deleted.", id);
        
        return ResultT<Guid>.Success(review.ReviewId ?? Guid.Empty);
    }

    public async Task<ResultT<double>> GetAverageRatingAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var registeredCompany = await _registeredCompanyRepository.GetByIdAsync(companyId, cancellationToken);
        if (registeredCompany == null)
        {
            _logger.LogError("Company with ID {CompanyId} was not found.", companyId);
            
            return ResultT<double>.Failure(Error.NotFound("404", $"Company with ID {companyId} was not found."));
        }
        
        var result = await _reviewsRepository.GetAverageRatingAsync(companyId, cancellationToken);
        
        _logger.LogInformation("Retrieved average rating for company with ID {CompanyId}.", companyId);
        
        return ResultT<double>.Success(result);
    }
}