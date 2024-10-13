using System.IO;

using Caliburn.Micro;
using As.Applications.Data;
using As.Applications.Loggers;
using As.Applications.ViewModels;

namespace As.Applications.Models
{
    internal static class TestModel
    {
        static readonly ILogger log = (ILogger)LogManager.GetLog(typeof(MainViewModel));

        public static bool Generate()
        {
            try
            {
                var c = Config.AppConfig;
                var calculations_valid = true;

                List<string> content = [];
                List<string> routines = [];

                double X = 0;
                double Y = 0;
                double Xmax = 0;
                double Ymax = 0;

                var power_min = c.PowerMinimum;
                var power_max = c.PowerMaximum;
                var speed_min = c.SpeedMinimum * 60;
                var speed_max = c.SpeedMaximum * 60;

                int group_count = c.GrayScale.GroupCount;

                routines.Add("O100   ( subroutine begin label 100, run one line from current point. )");
                var mode = c.GrayScale.Mode.Covariant();
                switch (mode)
                {
                    case Mode.Power:
                        routines.Add($"       ( Y Inner loop: scale {mode}; " +
                            $" Y = 0 - {c.GrayScale.Y.Range.Ci()}; " +
                            $" Q = {c.GrayScale.Y.First.Ci()} - {c.GrayScale.Y.Last.Ci()}, step {c.GrayScale.Y.Step.Ci()} in max Watt:255 )");
                        foreach (var p in c.GrayScale.Y)
                        {
                            var v = (int)p;
                            if (v < power_min) v = power_min;
                            if (power_max < v) v = power_max;
                            Y += c.GrayScale.Y.Increment;
                            if (Ymax < Y) Ymax = Y;
                            routines.Add($"M10 Q{v}");
                            routines.Add($"G1 Y{Y.Ci()}");
                        }
                        routines.Add("M10 Q0");
                        break;
                    case Mode.Speed:
                        routines.Add($"       ( Y Inner loop: scale {mode}; " +
                            $" Y = 0 - {c.GrayScale.Y.Range.Ci()}; " +
                            $" F = {(c.GrayScale.Y.First * 60).Ci()} - {(c.GrayScale.Y.Last * 60).Ci()}, step {(c.GrayScale.Y.Step * 60).Ci()} in mm/min )");
                        foreach (var s in c.GrayScale.Y)
                        {
                            var v = s * 60;
                            if (v < speed_min) v = speed_min;
                            if (speed_max < v) v = speed_max;
                            Y += c.GrayScale.Y.Increment;
                            if (Ymax < Y) Ymax = Y;
                            routines.Add($"G1 Y{Y.Ci()} F{(int)v}");
                        }
                        break;
                    default:
                        calculations_valid = false;
                        routines.Add($"       ( Inner loop: scale {mode} not recognised. )");
                        break;
                }
                routines.Add("M99    ( return, subroutine label 100 end )");

                mode = c.GrayScale.Mode;
                switch (mode)
                {
                    case Mode.Power:
                        content.Add($"       ( X Outer loop: scale {mode}; " +
                            $" X = 0 - {c.GrayScale.X.GetGroupedRange(c.GrayScale.GroupCount, c.GrayScale.GroupGap).Ci()}; " +
                            $" Q = {c.GrayScale.X.First.Ci()} - {c.GrayScale.X.Last.Ci()}, step {c.GrayScale.X.Step.Ci()} in max Watt:255 )");
                        content.Add("M10 Q0 ( Enable laser, power 0 )");
                        content.Add("G00 X0 Y0 F4200 ( go home )");
                        foreach (var p in c.GrayScale.X)
                        {
                            var v = (int)p;
                            if (v < power_min) v = power_min;
                            if (power_max < v) v = power_max;
                            content.Add($"M10 Q{v}");
                            content.Add("M98 P100 L1");
                            content.Add("M10 Q0");
                            X += c.GrayScale.X.Increment;
                            if (--group_count <= 0)
                            {
                                group_count = c.GrayScale.GroupCount;
                                X += c.GrayScale.GroupGap;
                            }
                            if (Xmax < X) Xmax = X;
                            content.Add($"G00 X{X.Ci()} Y0 F4200");
                        }
                        break;
                    case Mode.Speed:
                        content.Add($"       ( X Outer loop: scale {mode}; " +
                            $" X = 0 - {c.GrayScale.X.GetGroupedRange(c.GrayScale.GroupCount, c.GrayScale.GroupGap).Ci()}; " +
                            $" F = {(c.GrayScale.X.First * 60).Ci()} - {(c.GrayScale.X.Last * 60).Ci()}, step {(c.GrayScale.X.Step * 60).Ci()} in mm/min )");
                        content.Add("M10 Q0 ( Enable laser, power 0 )");
                        content.Add("G00 X0 Y0 F4200 ( go home )");
                        foreach (var s in c.GrayScale.X)
                        {
                            var v = s * 60;
                            if (v < speed_min) v = speed_min;
                            if (speed_max < v) v = speed_max;
                            content.Add($"G1 F{(int)v}");
                            content.Add("M98 P100 L1");
                            X += c.GrayScale.X.Increment;
                            if (--group_count <= 0)
                            {
                                group_count = c.GrayScale.GroupCount;
                                X += c.GrayScale.GroupGap;
                            }
                            if (Xmax < X) Xmax = X;
                            content.Add($"G00 X{X.Ci()} Y0 F4200");
                        }
                        break;
                    default:
                        calculations_valid = false;
                        content.Add($"       ( Outer loop: scale {mode} not recognised. )");
                        break;
                }

                using (var s = new StreamWriter(c.Testscript))
                {
                    s.WriteLine($"( This file is generated by {Config.AppName} v{Config.AppVersion} at {DateTime.Now:yyyy-MM-dd HH:mm} )");
                    s.WriteLine($"( Test and check the content before you use it. Use at your own risk, protect your eyes! )");
                    s.WriteLine();

                    if (!string.IsNullOrWhiteSpace(c.GrayScale.Intro))
                    {
                        s.WriteLine(c.GrayScale.Intro);
                        s.WriteLine();
                    }

                    s.WriteLine($"( This test uses X from 0 to {Xmax.Ci()} and Y from 0 to {Ymax.Ci()} )");
                    s.WriteLine();

                    if (!string.IsNullOrWhiteSpace(c.GrayScale.Header))
                    {
                        s.WriteLine(c.GrayScale.Header);
                        s.WriteLine();
                    }

                    if (calculations_valid)
                    {
                        foreach (var l in content) s.WriteLine(l);
                    }
                    else
                    {
                        s.WriteLine("( Calculations not valid, check LaserGrayscales configuration. )");
                    }

                    if (!string.IsNullOrWhiteSpace(c.GrayScale.Footer))
                    {
                        s.WriteLine();
                        s.WriteLine(c.GrayScale.Footer);
                    }

                    if (calculations_valid && 0 < routines.Count)
                    {
                        s.WriteLine();
                        foreach (var l in routines) s.WriteLine(l);
                    }
                }
                return calculations_valid;
            }
            catch (Exception x)
            {
                log.ErrorFormat($"Generate: fail; {x.Message.Trim()}", x);
                return false;
            }
        }
    }
}
