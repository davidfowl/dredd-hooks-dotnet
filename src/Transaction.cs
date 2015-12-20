namespace dredd_hooks_dotnet
{
  public class Transaction 
  {
    public string uuid { get; set; }
    public EventType Event { get; set; }
    public object Data { get; set; }
  }
}