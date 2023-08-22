using RimWorld;
using Verse;

namespace MapGenEvents
{

    internal class MapIncidentCreator : IExposable
    {
        private IMapIncidentCreatorPrototype _prototype;
        private Map _map;
        private bool _isFinished;
        private float _occurTick;

        private float IntervalTicks => _prototype.IntervalTicks;
        private IncidentDef Incident => _prototype.Incident;
        private bool Repeat => _prototype.Repeat;

        public MapIncidentCreator()
        {
        }

        public MapIncidentCreator(IMapIncidentCreatorPrototype prototype, Map map)
        {
            _prototype = prototype;
            _map = map;
            _occurTick = Find.TickManager.TicksGame + prototype.DelayTicks;
            _isFinished = false;
        }

        public void Tick()
        {
            if (Find.AnyPlayerHomeMap == null || _isFinished)
            {
                return;
            }

            if (Find.TickManager.TicksGame >= _occurTick)
            {
                IncidentParms parms = StorytellerUtility.DefaultParmsNow(Incident.category, _map);
                if (!Incident.Worker.TryExecute(parms))
                {
                    _isFinished = true;
                }
                else if (Repeat && IntervalTicks > 0f)
                {
                    _occurTick += IntervalTicks;
                }
                else
                {
                    _isFinished = true;
                }
            }
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref _prototype, "prototype");
            Scribe_References.Look(ref _map, "map");
            Scribe_Values.Look(ref _isFinished, "isFinished", defaultValue: false);
            Scribe_Values.Look(ref _occurTick, "occurTick", defaultValue: 0f);
        }
    }

}