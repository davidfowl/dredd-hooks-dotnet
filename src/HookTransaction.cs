namespace dredd_hooks_dotnet
{
  public struct HookTransaction
  {
    public string uuid { get; set; }
    public string @event { get; set; }
    public Transaction[] data { get; set; }
  }
}
