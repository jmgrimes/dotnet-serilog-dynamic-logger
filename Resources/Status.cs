namespace LaunchDarkly.DynamicLogger.Resources;

public class Status
{
    public bool Initialized { get; set; }
    public bool ClientSdkSupported { get; set; }
    public bool MobileSdkSupported { get; set; }

    public Status(bool initialized, bool clientSdkSupported, bool mobileSdkSupported)
    {
        Initialized = initialized;
        ClientSdkSupported = clientSdkSupported;
        MobileSdkSupported = mobileSdkSupported;
    }
}