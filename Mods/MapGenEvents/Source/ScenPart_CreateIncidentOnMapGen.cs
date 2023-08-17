using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace MapGenEvents
{

    internal class ScenPart_CreateIncidentOnMapGen : ScenPart_IncidentBase, IMapIncidentCreatorPrototype
    {
        private const float IntervalMidpoint = 30f;
        private const float IntervalDeviation = 15f;

        [CanBeNull] private string _uniqueLoadID;

        private float _intervalDays;

        [CanBeNull] private string _intervalDaysBuffer;

        private bool _repeat;

        private bool _onPermanentMaps;

        private bool _onTemporaryMaps;

        protected override string IncidentTag => "CreateIncidentOnMapGen";

        public float IntervalTicks => 60000f * _intervalDays;

        public bool Repeat => _repeat;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _uniqueLoadID, "uniqueLoadID");
            Scribe_Values.Look(ref _intervalDays, "intervalDays");
            Scribe_Values.Look(ref _repeat, "repeat", defaultValue: false);
            Scribe_Values.Look(ref _onPermanentMaps, "onPermanentMaps", defaultValue: true);
            Scribe_Values.Look(ref _onTemporaryMaps, "onTemporaryMaps", defaultValue: false);
        }

        public override int GetHashCode()
        {
            return GetUniqueLoadID().GetHashCode();
        }

        public string GetUniqueLoadID()
        {
            if (string.IsNullOrEmpty(_uniqueLoadID))
            {
                _uniqueLoadID = "ScenPart_CreateIncidentOnMapGen_" + GenText.RandomSeedString();
            }

            return _uniqueLoadID;
        }

        public override bool TryMerge(ScenPart other) => false;

        public override bool CanCoexistWith(ScenPart other) => true;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            float fieldCount = 5f;
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * fieldCount);
            Rect rect = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height / fieldCount);
            Rect rect2 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height / fieldCount, scenPartRect.width,
                scenPartRect.height / fieldCount);
            Rect rect3 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height * 2f / fieldCount,
                scenPartRect.width, scenPartRect.height / fieldCount);
            Rect rect4 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height * 3f / fieldCount,
                scenPartRect.width, scenPartRect.height / fieldCount);
            Rect rect5 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height * 4f / fieldCount,
                scenPartRect.width, scenPartRect.height / fieldCount);
            DoIncidentEditInterface(rect);
            Widgets.TextFieldNumericLabeled(rect2, "intervalDays".Translate(), ref _intervalDays,
                ref _intervalDaysBuffer);
            Widgets.CheckboxLabeled(rect3, "repeat".Translate(), ref _repeat);
            Widgets.CheckboxLabeled(rect4, "onPermanentMaps".Translate(), ref _onPermanentMaps);
            Widgets.CheckboxLabeled(rect5, "onTemporaryMaps".Translate(), ref _onTemporaryMaps);
        }

        public override void Randomize()
        {
            base.Randomize();
            _intervalDays = Math.Max(0f, IntervalDeviation * Rand.Gaussian() + IntervalMidpoint);
            _repeat = Rand.Range(0, 100) < 50;
            int onWhichMaps = Rand.RangeInclusive(1, 3);
            _onPermanentMaps = onWhichMaps <= 2;
            _onTemporaryMaps = onWhichMaps >= 2;
        }

        protected override IEnumerable<IncidentDef> RandomizableIncidents()
        {
            yield return IncidentDefOf.Eclipse;
            yield return IncidentDefOf.ToxicFallout;
            yield return IncidentDefOf.SolarFlare;
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);
            if ((_onPermanentMaps && map.IsPlayerHome) || (_onTemporaryMaps && !map.IsPlayerHome))
            {
                map.GetComponent<CreateIncidentMapComponent>().Start(this);
            }
        }

        public MapIncidentCreator SpawnMapIncidentCreator(Map map)
        {
            return new MapIncidentCreator(this, map);
        }
    }

}