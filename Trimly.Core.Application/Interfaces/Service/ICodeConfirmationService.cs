namespace Trimly.Core.Application.Interfaces.Service;

public interface ICodeConfirmationService
{
    Task<string> GenerateCodeConfirmation(string emailAddress);
}