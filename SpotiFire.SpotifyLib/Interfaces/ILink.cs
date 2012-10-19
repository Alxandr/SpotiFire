namespace SpotiFire
{
    public interface ILink : ISpotifyObject
    {
        LinkType Type { get; }
        object Object { get; }
    }

    public interface ILink<out T> : ILink
    {
        T Object { get; }
    }
}
