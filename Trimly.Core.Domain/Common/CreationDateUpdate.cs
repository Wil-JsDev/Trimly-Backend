
namespace Trimly.Core.Domain.Common
{
    public class CreationDateUpdate
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateAt { get; set; }
    }
}
