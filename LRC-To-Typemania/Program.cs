using Coosu.Beatmap;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Mathematics;
using System;
using System.Globalization;
using System.Text;

namespace LRC_To_Typemania
{
    internal class Program
    {
        static Dictionary<char, int> cols = new Dictionary<char, int>();

        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            cols.Add('q', 0);
            cols.Add('a', 20);
            cols.Add('z', 40);
            cols.Add('w', 60);
            cols.Add('s', 79);
            cols.Add('x', 99);
            cols.Add('e', 119);
            cols.Add('d', 138);
            cols.Add('c', 158);
            cols.Add('r', 178);
            cols.Add('f', 197);
            cols.Add('v', 217);
            cols.Add('t', 237);
            cols.Add('g', 257);
            cols.Add('b', 276);
            cols.Add('y', 296);
            cols.Add('h', 316);
            cols.Add('n', 335);
            cols.Add('u', 355);
            cols.Add('j', 375);
            cols.Add('m', 394);
            cols.Add('i', 414);
            cols.Add('k', 434);
            cols.Add('o', 453);
            cols.Add('l', 473);
            cols.Add('p', 493);

            OsuFile osuFile = OsuFile.CreateEmpty();
            MetadataSection metadata = osuFile.Metadata;
            HitObjectSection hitObjects = osuFile.HitObjects;

            metadata.Creator = "KillBottt";
            metadata.Version = "Normal";

            foreach (string line in File.ReadLines(@"E:\Media\Downloads\Alan Walker - The Spectre - Advance (English).lrc"))
            {
                if (line.Length >= 11 && int.TryParse(line.Substring(1, 2), out int _))
                {
                    string[] lineParts = line.Split(']');
                    lineParts[0] = lineParts[0][1..];
                    DateTime time = DateTime.ParseExact(lineParts[0], "mm:ss.ff", null);
                    TimeSpan timeSpan = TimeSpan.ParseExact(lineParts[0], @"mm\:ss\.ff", null);

                    StringBuilder sb = new StringBuilder();
                    foreach (char c in lineParts[1])
                    {
                        if (cols.Keys.Contains(c))
                        {
                            sb.Append(c);
                        }
                    }

                    foreach (char c in sb.ToString())
                    {
                        RawHitObject hitObject = new RawHitObject();
                        hitObject.Y = 192;
                        hitObject.X = 486; // TODO
                        hitObject.Offset = (int) timeSpan.TotalMilliseconds;

                        hitObjects.HitObjectList.Add(hitObject);
                    }
                }
                else if (line.Contains("ar:"))
                {
                    metadata.Artist = RemoveBrackets(line).Replace("ar:", "").Trim();
                }
                else if (line.Contains("ti:"))
                {
                    metadata.Title = RemoveBrackets(line).Replace("ti:", "").Trim();
                }
                else if (line.Contains("al:"))
                {
                    metadata.Source = RemoveBrackets(line).Replace("al:", "").Trim();
                }
            }
        }

        string RemoveBrackets(string line)
        {
            return line.Replace("[", "").Replace("]", "");
        }

        void Example()
        {
            OsuFile osuFile = OsuFile.CreateEmpty();

            // [General]
            GeneralSection general = osuFile.General;
            // original key-value pair
            Console.WriteLine(string.Join(',',
                general.AudioFilename,
                general.AudioLeadIn,
                general.Countdown,
                general.EpilepsyWarning,
                general.LetterboxInBreaks,
                general.Mode,
                general.PreviewTime,
                general.SampleSet,
                general.SkinPreference,
                general.StackLeniency,
                general.WidescreenStoryboard
            ));

            // [Difficulty]
            DifficultySection difficulty = osuFile.Difficulty;
            // original key-value pair
            Console.WriteLine(string.Join(',',
                difficulty.CircleSize,
                difficulty.SliderTickRate,
                difficulty.ApproachRate,
                difficulty.HpDrainRate,
                difficulty.OverallDifficulty,
                difficulty.SliderMultiplier
            ));

            // [Metadata]
            MetadataSection metadata = osuFile.Metadata;
            // original key-value pair
            Console.WriteLine(string.Join(',',
                metadata.Artist,
                metadata.ArtistUnicode,
                metadata.Title,
                metadata.TitleUnicode,
                metadata.BeatmapId,
                metadata.BeatmapSetId,
                metadata.Creator,
                metadata.Source,
                string.Join(' ', metadata.TagList),
                metadata.Version
            ));
            // extension
            metadata.ArtistMeta.ToPreferredString(); // return origin string if unicode is null or empty.
            metadata.TitleMeta.ToPreferredString();

            // [Editor]
            EditorSection editor = osuFile.Editor;
            // original key-value pair
            Console.WriteLine(string.Join(',',
                editor.BeatDivisor,
                string.Join(',', editor.Bookmarks),
                editor.DistanceSpacing,
                editor.GridSize,
                editor.TimelineZoom
            ));

            // [Colour]
            ColorSection color = osuFile.Colours;
            // original key-value pair
            Console.WriteLine(string.Join(',',
              color.Combo1,
              color.Combo2,
              color.Combo3,
              color.Combo4,
              color.Combo5,
              color.Combo6,
              color.Combo7,
              color.Combo8
            ));

            // [Timing]
            TimingSection timings = osuFile.TimingPoints;
            // original timing points option
            TimingPoint timingPoint = timings[0];
            Console.WriteLine(string.Join(',',
                timingPoint.Offset,
                timingPoint.Factor,
                timingPoint.Inherit,
                timingPoint.Kiai,
                timingPoint.Rhythm,
                timingPoint.TimingSampleset,
                timingPoint.Track,
                timingPoint.Volume
            ));
            // timing point extension
            Console.WriteLine($"{timingPoint.Multiple},{timingPoint.Bpm}");
            // timing calculate extension
            Dictionary<double, double> all1_1RhythmIntervalInDifferentBpms = timings.GetInterval(1);
            foreach (var kv in all1_1RhythmIntervalInDifferentBpms)
            {
                var startOffset = kv.Key;
                var bpm1_1interval = kv.Value;
                Console.WriteLine($"{startOffset},{bpm1_1interval}");
            }
            // returns current or closest previous timing point.
            // e.g: (12345) or (12300 when no timing point on 12345)
            TimingPoint line = timings.GetLine(12345);
            // just like above, but only select red line
            TimingPoint redLine = timings.GetRedLine(12345);
            // returns all kiai parts
            RangeValue<double>[] kiaiRanges = timings.GetTimingKiais();

            // [HitObjects]
            HitObjectSection hitObjects = osuFile.HitObjects;
            RawHitObject hitObject = hitObjects[0];
            // original hit objects option
            Console.WriteLine(string.Join(',',
                hitObject.Offset,
                hitObject.X,
                hitObject.Y,
                hitObject.RawType,
                hitObject.Hitsound,
                hitObject.Extras
            ));
            // object type and NC information
            HitObjectType trueType = hitObject.ObjectType;
            int ncSkip = hitObject.NewComboCount;
            // object extras
            Console.WriteLine(string.Join(',',
                hitObject.SampleSet,
                hitObject.AdditionSet,
                hitObject.CustomIndex,
                hitObject.SampleVolume
            ));
            // mania slider / spinner
            if (trueType == HitObjectType.Hold || trueType == HitObjectType.Spinner)
            {
                int endTime = hitObject.HoldEnd;
            }
            // std slider
            if (trueType == HitObjectType.Slider)
            {
                SliderInfo slider = hitObject.SliderInfo;

                SliderType sliderType = slider.SliderType; // linear, bezier ...
                Vector2<float> startPoint = slider.StartPoint; // slider.StartPoint = (hitObject.X, hitObject.Y)
                Vector2<float> endPoint = slider.EndPoint;
                decimal pixelLength = slider.PixelLength; // raw data

                int repeat = slider.Repeat; // 1 = won't repeat
                Vector2<float>[] curvePoints = slider.CurvePoints; // doesn't include start point

                double startTime = slider.StartTime;
                double endTime = slider.EndTime;

                SliderEdge[] edges = slider.Edges;
                foreach (var sliderEdge in edges)
                {
                    Vector2<float> edgePosition = sliderEdge.Point;
                    double edgeOffset = sliderEdge.Offset;
                    ObjectSamplesetType edgeSample = sliderEdge.EdgeSample;
                    ObjectSamplesetType edgeAddition = sliderEdge.EdgeAddition;
                    HitsoundType edgeHitsound = sliderEdge.EdgeHitsound;
                }

                SliderTick[] ticks = slider.Ticks;
                SliderTick[] defaultTrial = slider.BallTrail;
                SliderTick[] customTrial = slider.GetDiscreteSliderTrailData(100);
                foreach (var sliderTick in ticks)
                {
                    double offset = sliderTick.Offset;
                    Vector2<float> position = sliderTick.Point;
                }
            }
        }
    }
}