using System.Text.Json.Nodes;
using SemBackDotNet10.actionDomain.controller.dto;

namespace SemBackDotNet10.actionDomain.service.interf;

public interface ICallActionRestApiService
{
    string? getToken(string? userName, string? password);
    JsonObject? startActionProcess(StartActionDTO body);
    JsonObject? getRoyalVisionCorrectionAct(long? id);
}
