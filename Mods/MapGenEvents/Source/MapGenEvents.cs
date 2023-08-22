using System;

namespace MapGenEvents
{

    [Verse.StaticConstructorOnStartup]
    public static class MapGenEvents
    {
        public static readonly Version Version = new Version(1, 1, 3);
        
        static MapGenEvents()
        {
            Verse.Log.Message("[MapGenEvents] Version " + Version + " loaded.");
        }
    }

}