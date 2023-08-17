using RimWorld;
using Verse;

namespace MapGenEvents
{

    internal interface IMapIncidentCreatorPrototype : ILoadReferenceable
    {
        MapIncidentCreator SpawnMapIncidentCreator(Map map);
        float IntervalTicks { get; }
        IncidentDef Incident { get; }
        bool Repeat { get; }
    }

}