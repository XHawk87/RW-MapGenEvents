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
        private const float DelayMidpoint = 30f;
        private const float DelayDeviation = 15f;
        private const float IntervalMidpoint = 30f;
        private const float IntervalDeviation = 15f;
        [CanBeNull] private string _uniqueLoadID;
        private float _delayDays;
        [CanBeNull] private string _delayDaysBuffer;
        private float _intervalDays;
        [CanBeNull] private string _intervalDaysBuffer;
        private bool _repeat;
        private bool _onPermanentMaps;
        private bool _onTemporaryMaps;
        private float _scenPartRectHeight;

        protected override string IncidentTag => "CreateIncidentOnMapGen";

        public float DelayTicks => 60000f * _delayDays;

        public float IntervalTicks => 60000f * _intervalDays;

        public bool Repeat => _repeat;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _uniqueLoadID, "uniqueLoadID");
            Scribe_Values.Look(ref _delayDays, "delayDays");
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

        private Rect GetScenPartRect(Listing_ScenEdit listing)
        {
            var rect = listing.GetScenPartRect(this, _scenPartRectHeight);
            _scenPartRectHeight = 0f;
            return rect;
        }

        private Rect GetRowRect(Listing_Standard listing)
        {
            _scenPartRectHeight += RowHeight;
            return listing.GetRect(RowHeight);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Listing_Standard subListing = new Listing_Standard();
            subListing.Begin(GetScenPartRect(listing));
            DoIncidentEditInterface(GetRowRect(subListing));
            Widgets.TextFieldNumericLabeled(GetRowRect(subListing),
                "delayDays".Translate(), ref _delayDays, ref _delayDaysBuffer);
            Widgets.CheckboxLabeled(GetRowRect(subListing), "repeat".Translate(), ref _repeat);
            if (_repeat)
            {
                Widgets.TextFieldNumericLabeled(GetRowRect(subListing),
                    "intervalDays".Translate(), ref _intervalDays, ref _intervalDaysBuffer);
            }

            Widgets.CheckboxLabeled(GetRowRect(subListing), "onPermanentMaps".Translate(), ref _onPermanentMaps);
            Widgets.CheckboxLabeled(GetRowRect(subListing), "onTemporaryMaps".Translate(), ref _onTemporaryMaps);
            subListing.End();
        }

        public override void Randomize()
        {
            base.Randomize();
            _delayDays = Math.Max(0f, DelayDeviation * Rand.Gaussian() + DelayMidpoint);
            _repeat = Rand.Range(0, 100) < 50;
            _intervalDays = Math.Max(0f, IntervalDeviation * Rand.Gaussian() + IntervalMidpoint);
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