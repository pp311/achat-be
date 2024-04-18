using AChat.Domain;

namespace AChat.Application.ViewModels;

public class PagingRequest : IRequest
{
    public int PageNumber { get; set; } = AppConstant.DefaultPageNumber;
    
    public int PageSize { get; set; } = AppConstant.DefaultPageSize;
    
    public bool IsDescending { get; set; } = false;
}
