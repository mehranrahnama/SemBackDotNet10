namespace SemBackDotNet10.basicDomain.service;

public interface IPoopakService
{
    object? SendMessageToHOSCOPoopak(PoopakSendMessageDto sendMessageDto);
    object? SendMessageToIRISAPoopak(PoopakSendMessageDto sendMessageDto);
    object? SendSMSMessage(ERPSendMessageDto message);
}
