using Common.model.entity;
namespace Common.repository;
public sealed class StoredFileRepository(IPersistenceSession<StoredFile> session) : EntityRepository<StoredFile,long>(session,x=>x.FileId);
