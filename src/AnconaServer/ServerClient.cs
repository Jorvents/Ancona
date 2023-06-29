using System.Net;
using System.Numerics;

namespace AnconaServer;

public class ServerClient
{
    public EndPoint endpoint;
    public Guid id;

    public Vector2 loc;
    public int testint;

    public ServerClient(EndPoint endpoint, Guid id)
    {
        loc = Vector2.Zero;
        this.endpoint = endpoint;
        this.id = id;
        testint = 0;
    }
}