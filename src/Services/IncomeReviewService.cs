using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.IncomeReviewModels;
using CashTrack.Repositories.Common;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeReviewService;


public interface IIncomeReviewService
{
    Task<IncomeReviewListItem> GetIncomeReviewByIdAsync(int id);
    Task<IncomeReviewResponse> GetIncomeReviewsAsync(IncomeReviewRequest request);
    Task<bool> UpdateIncomeReviewAsync(int id);
}

public class IncomeReviewService : IIncomeReviewService
{
    private readonly IRepository<IncomeReview> _repo;
    private readonly IMapper _mapper;

    public IncomeReviewService(IRepository<IncomeReview> repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

    public async Task<IncomeReviewListItem> GetIncomeReviewByIdAsync(int id)
    {
        var singleIncome = await _repo.FindById(id);
        return _mapper.Map<IncomeReviewListItem>(singleIncome);
    }

    public async Task<IncomeReviewResponse> GetIncomeReviewsAsync(IncomeReviewRequest request)
    {
        var income = await _repo.FindWithPagination(x => x.is_reviewed == false, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(x => x.is_reviewed == false);

        return new IncomeReviewResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeReviewListItem[]>(income));
    }

    public async Task<bool> UpdateIncomeReviewAsync(int id)
    {
        //only sets it to reviewed... nothing else
        //might have bugs here... if so take off change tracker in repo
        var income = await _repo.FindById(id);
        if (income == null)
            throw new IncomeNotFoundException(id.ToString());

        income.is_reviewed = true;
        return await _repo.Update(income);
    }
}

public class IncomeReviewMapper : Profile
{
    public IncomeReviewMapper()
    {
        CreateMap<IncomeReview, IncomeReviewListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.id))
            .ForMember(x => x.Date, o => o.MapFrom(src => src.date))
            .ForMember(x => x.Amount, o => o.MapFrom(src => src.amount))
            .ForMember(x => x.Notes, o => o.MapFrom(src => src.notes))
            .ForMember(x => x.SuggestedCategoryId, o => o.MapFrom(src => src.suggested_category.id))
            .ForMember(x => x.SuggestedCategory, o => o.MapFrom(src => src.suggested_category.category))
            .ForMember(x => x.SuggestedSourceId, o => o.MapFrom(src => src.suggested_source.id))
            .ForMember(x => x.SuggestedSource, o => o.MapFrom(src => src.suggested_source.source))
            .ReverseMap();
    }
}


