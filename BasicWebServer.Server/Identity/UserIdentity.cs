﻿namespace BasicWebServer.Server.Identity
{
    public class UserIdentity
    {
        public string Id { get; init; }

        public bool IsAuthenticated => this.Id != null;

        public string GetId()
        { return Id; }
    }

    
}
