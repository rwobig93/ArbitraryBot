namespace ArbitraryBot.Shared
{
    public enum StatusReturn
    {
        NotFound,
        Found,
        Failure,
        Success
    }

    public enum AppFile
    {
        Config,
        Log,
        SavedData
    }

    public enum Alert
    {
        Email,
        Webhook,
        Email_Webhook
    }

    public enum TrackInterval
    {
        OneMin,
        FiveMin
    }
}
