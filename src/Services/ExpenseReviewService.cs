using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Repositories.Common;
using System.Threading.Tasks;

namespace CashTrack.Services.ExpenseReviewService;

public interface IExpenseReviewService
{
    Task<ExpenseReviewListItem> GetExpenseReviewByIdAsync(int id);
    Task<ExpenseReviewResponse> GetExpenseReviewsAsync(ExpenseReviewRequest request);
    Task<bool> UpdateExpenseReviewAsync(int id);
}

public class ExpenseReviewService : IExpenseReviewService
{
    private readonly IRepository<ExpenseReview> _repo;
    private readonly IMapper _mapper;

    public ExpenseReviewService(IRepository<ExpenseReview> repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

    public async Task<ExpenseReviewListItem> GetExpenseReviewByIdAsync(int id)
    {
        var singleExpense = await _repo.FindById(id);
        return _mapper.Map<ExpenseReviewListItem>(singleExpense);
    }

    public async Task<ExpenseReviewResponse> GetExpenseReviewsAsync(ExpenseReviewRequest request)
    {
        var expenses = await _repo.FindWithPagination(x => true, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(x => x.is_reviewed == false);

        return new ExpenseReviewResponse(request.PageNumber, request.PageSize, count, _mapper.Map<ExpenseReviewListItem[]>(expenses));
    }

    public async Task<bool> UpdateExpenseReviewAsync(int id)
    {
        //only sets it to reviewed... nothing else
        //might have bugs here... if so take off change tracker in repo
        var expense = await _repo.FindById(id);
        if (expense == null)
            throw new ExpenseNotFoundException(id.ToString());

        expense.is_reviewed = true;
        return await _repo.Update(expense);
    }
}

public class ExpenseReviewMapper : Profile
{
    public ExpenseReviewMapper()
    {
        CreateMap<ExpenseReview, ExpenseReviewListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Date, o => o.MapFrom(src => src.date))
            .ForMember(x => x.Amount, o => o.MapFrom(src => src.amount))
            .ForMember(x => x.Notes, o => o.MapFrom(src => src.notes))
            .ForMember(x => x.SuggestedCategoryId, o => o.MapFrom(src => src.suggested_category.Id))
            .ForMember(x => x.SuggestedCategory, o => o.MapFrom(src => src.suggested_category.sub_category_name))
            .ForMember(x => x.SuggestedMerchantId, o => o.MapFrom(src => src.suggested_merchant.Id))
            .ForMember(x => x.SuggestedMerchant, o => o.MapFrom(src => src.suggested_merchant.name))
            .ReverseMap();
    }
}
