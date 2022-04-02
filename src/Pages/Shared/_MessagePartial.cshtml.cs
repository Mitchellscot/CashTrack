namespace CashTrack.Pages.Shared
{
    public class _MessagePartial
    {
        public string Message { get; set; }
        public MessageType MessageType { get; set; }

    }
    public enum MessageType
    {
        Info,
        Danger,
        Success
    }
}