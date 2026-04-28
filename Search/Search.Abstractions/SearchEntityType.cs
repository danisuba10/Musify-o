namespace Search.Abstractions;

public enum SearchEntityType : byte
{
    Song     = 0,
    Artist   = 1,
    Album    = 2,
    Playlist = 3,
    User     = 4,
    All      = 255
}
