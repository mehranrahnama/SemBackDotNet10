// C# migration of config/storage/StorageProperties.java

namespace Common.config.storage;

public class StorageProperties { public const string SectionName="storage"; public string Location { get; set; } = "Upload"; }
