#nullable enable
using System.Collections.Generic;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class ERPSendMessageDto
{
    public List<string?>? PhoneNumbers { get; set; }
    public string? Message { get; set; }

    public ERPSendMessageDto() { }

    public ERPSendMessageDto(List<string?>? phoneNumbers, string? message)
    {
        PhoneNumbers = phoneNumbers;
        Message = message;
    }
}
