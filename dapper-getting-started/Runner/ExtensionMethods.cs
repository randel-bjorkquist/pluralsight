using YamlDotNet.Serialization;

namespace Runner;

internal static class ExtensionMethods
{
  public static void Output(this object item)
  {
    var serializer = new SerializerBuilder().Build();
    var yaml = serializer.Serialize(item);
    Console.WriteLine(yaml);
  }
}
