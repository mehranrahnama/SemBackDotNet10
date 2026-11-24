namespace SemBackDotNet10.basicDomain.Events
{
    public class AssignmentChangedEvent
    {
        public long LevelId { get; }
        public long? NewFacilitatorId { get; }
        public long? NewManagerId { get; }

        public AssignmentChangedEvent(long levelId, long? newFacilitatorId, long? newManagerId)
        {
            LevelId = levelId;
            NewFacilitatorId = newFacilitatorId;
            NewManagerId = newManagerId;
        }
    }
}
