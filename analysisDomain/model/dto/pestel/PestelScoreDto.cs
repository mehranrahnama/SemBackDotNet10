namespace SemBackDotNet10.analysisDomain.model.dto.pestel;

public class PestelScoreDto
{
    public long? Id { get; set; }
    public PestelDto? PestelDto { get; set; }
    public long? PlanId { get; set; }
    public string? OpportunityOrThreat { get; set; }
    public int CurrentScore { get; set; }
    public int FutureScore { get; set; }
    public int PreviousYearCurrentScore { get; set; }
    public int PreviousYearFutureScore { get; set; }

    public PestelScoreDto()
    {
    }

    public PestelScoreDto(long? id, PestelDto? pestelDto, long? planId, string? opportunityOrThreat,
        int currentScore, int futureScore, int previousYearCurrentScore, int previousYearFutureScore)
    {
        Id = id;
        PestelDto = pestelDto;
        PlanId = planId;
        OpportunityOrThreat = opportunityOrThreat;
        CurrentScore = currentScore;
        FutureScore = futureScore;
        PreviousYearCurrentScore = previousYearCurrentScore;
        PreviousYearFutureScore = previousYearFutureScore;
    }

    public PestelScoreDto(long? id, long? pestelId, long? pestelGroupId, string? pestelGroupTitle, string? pestelTitle,
        long? planId, string? opportunityOrThreat, int currentScore, int futureScore,
        int previousYearCurrentScore, int previousYearFutureScore)
    {
        Id = id;
        PestelDto = new PestelDto(pestelId, pestelGroupId, pestelGroupTitle, pestelTitle);
        PlanId = planId;
        OpportunityOrThreat = opportunityOrThreat;
        CurrentScore = currentScore;
        FutureScore = futureScore;
        PreviousYearCurrentScore = previousYearCurrentScore;
        PreviousYearFutureScore = previousYearFutureScore;
    }
}
