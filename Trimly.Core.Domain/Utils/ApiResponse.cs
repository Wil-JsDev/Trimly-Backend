using System.Net;

namespace Trimly.Core.Domain.Utils;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static ApiResponse<T> SuccessResponse(T data) =>
        new ApiResponse<T> { Success = true, Data = data };
    public static ApiResponse<T> ErrorResponse(string errormessage) =>
        new ApiResponse<T>{Success = false, ErrorMessage = errormessage};
}