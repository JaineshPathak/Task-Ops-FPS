using System.Collections.Generic;
using System.Linq;

public static class SniperPlayerRegistry
{
    static List<SniperPlayerObject> players = new List<SniperPlayerObject>();

    static SniperPlayerObject CreatePlayer(BoltConnection connection)
    {
        SniperPlayerObject player;

        player = new SniperPlayerObject();
        player.connection = connection;

        if (player.connection != null)
        {
            player.connection.UserData = player;
        }

        players.Add(player);

        return player;
    }

    public static IEnumerable<SniperPlayerObject> AllPlayers
    {
        get { return players; }
    }

    public static SniperPlayerObject ServerPlayer
    {
        get { return players.First(player => player.IsServer); }
    }

    public static SniperPlayerObject CreateServerPlayer()
    {
        return CreatePlayer(null);
    }

    public static SniperPlayerObject CreateClientPlayer(BoltConnection connection)
    {
        return CreatePlayer(connection);
    }

    public static SniperPlayerObject GetPlayer(BoltConnection connection)
    {
        if (connection == null)
        {
            return ServerPlayer;
        }

        return (SniperPlayerObject)connection.UserData;
    }
}
