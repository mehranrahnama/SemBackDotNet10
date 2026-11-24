using Common.controller;
using Common.model.entity;
namespace Common.service.interf;
public interface CartableRoyalVisionService : ICartableRoyalVisionControllerService
{
    object LoadGridByObjectId(string? body,int page,int size,string? orderBy,string? direction);
    CartableRoyalVision FindByIdOrElseThrow(long? id);
    void Delete(string measureName);
}
