namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    public sealed class User
    {
        public long UserId { get; set; }

        public Guid? OldUserId { get; set; }

        public string? UserName { get; set; }

        public long? TenantId { get; set; }

        public string? NameF { get; set; }

        public string? FamilyF { get; set; }

        public string? NationalCode { get; set; }

        public int? OrganizationId { get; set; }

        public int? PositionId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public SemBackDotNet10.basicDomain.Model.Entity.Position? Position { get; set; }

        public int? PlaningLevelId { get; set; }

        public bool? Retired { get; set; }

        public bool? FlgRater { get; set; }

        public bool? MasterRater { get; set; }

        public bool? IsManager { get; set; }

        public string? CellPhoneNo { get; set; }

        public string? InternalPhoneNum { get; set; }

        public short? Type { get; set; }

        public int? FailedLoginAttempts { get; set; }

        public DateTime? LastFailedLoginAttempt { get; set; }

        public DateTime? LastSuccessfulLoginAttempt { get; set; }

        public bool? LoginDisabled { get; set; }

        public DateTime? LoginDisabledTime { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? Version { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? PersonnelNumber { get; set; }

        public ICollection<UserAccount> Accounts { get; set; }
            = new List<UserAccount>();
    }
}
