#nullable enable
using System.Collections.Generic;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class PoopakSendMessageDto
{
    public List<string?>? Users { get; set; }
    public string? Message { get; set; }

    public PoopakSendMessageDto() { }

    public PoopakSendMessageDto(List<string?>? users, string? message)
    {
        Users = users;
        Message = message;
    }
}
