namespace dredd_hooks_dotnet
{
  public struct Transaction
  {
    public string uuid { get; set; }
    public string Event { get; set; }
    public object Data { get; set; }
  }
}
