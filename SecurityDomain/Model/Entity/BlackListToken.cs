using SemBackDotNet10.SecurityDomain.Model.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    [Table("tblBlackListToken", Schema = "dbo")]
    public class BlackListToken : TimeStampEntity<long>
    {
        private long id;
        private UserAccount account;
        private string token;
        private DateTime expiryDate;
        private long? duration;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "int")]
        public override long Id
        {
            get => id;
            set => id = value;
        }

        [Column("account_id", TypeName = "int")]
        public long AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public UserAccount Account
        {
            get => account;
            set => account = value;
        }

        [Column("token")]
        [Required]
        public string Token
        {
            get => token;
            set => token = value;
        }

        [Column("expiryDate", TypeName = "datetime")]
        [Required]
        public DateTime ExpiryDate
        {
            get => expiryDate;
            set => expiryDate = value;
        }

        [NotMapped]
        public long? Duration
        {
            get => duration;
            set => duration = value;
        }
    }
}