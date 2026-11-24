namespace Common.config.resourcebundle;

public sealed class MessageSourceConfig
{
 public string BaseName { get; set; } = "messages.messages";
 public System.Reflection.Assembly ResourceAssembly { get; set; } = typeof(MessageSourceConfig).Assembly;
 public System.Resources.ResourceManager CreateMessageSource() => new(BaseName,ResourceAssembly);
}
