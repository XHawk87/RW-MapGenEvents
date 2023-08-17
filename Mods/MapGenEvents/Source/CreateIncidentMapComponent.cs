using System.Collections.Generic;
using Verse;

namespace MapGenEvents
{

    internal class CreateIncidentMapComponent : MapComponent
    {
        private List<MapIncidentCreator> _mapIncidentCreators = new List<MapIncidentCreator>();

        public CreateIncidentMapComponent(Map map) : base(map)
        {
        }

        public void Start(IMapIncidentCreatorPrototype prototype)
        {
            _mapIncidentCreators.Add(prototype.SpawnMapIncidentCreator(map));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _mapIncidentCreators, "mapIncidentCreators", LookMode.Deep);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            foreach (var mapIncidentCreator in _mapIncidentCreators)
            {
                mapIncidentCreator.Tick();
            }
        }
    }

}