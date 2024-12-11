using System.IO;

using As.Applications.Config;
using As.Applications.Converters;
using As.Applications.Data.Patterns;
using As.Applications.Data.Scales;
using As.Applications.IO;
using As.Applications.Loggers;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

using ILogger = As.Applications.Loggers.ILogger;
using LogManager = Caliburn.Micro.LogManager;

namespace As.Applications.Plotters.Engine
{
    public class Plotter
    {
        // TODO: must use config paths, now using fixed paths for building

        static readonly ILogger Log = (ILogger)LogManager.GetLog(typeof(Plotter));

        const string NAME_DEFAULT = "TestPattern";
        public const string EXTENSION = ".nc";
        public const string FILTER = $"GCode files |*{EXTENSION}|all|*.*";

        #region Properties
        public bool StoreIntermediate { get; set; } = true;

        public required string Name
        {
            get => !string.IsNullOrWhiteSpace(_name) ? _name : NAME_DEFAULT;
            set => _name = value;
        }
        string _name = "";

        public string FilePath { get; private set; } = "";

        public required TestPattern TestPattern { get; set; }

        public required string BuildPath { get; set; }

        readonly Context context = new()
        {
            Verbose = Settings.App.Verbose,
            Debug = Settings.App.Debug
        };
        #endregion Properties

        #region Actions
        bool TryProcess(out List<PlotAction> result)
        {
            result = TestPattern.Plot();
            var r = (result.Count != 0);
            if (r)
            {
                if (StoreIntermediate)
                {
                    var path = BuildFilePath("{0}.i.xml", (object)Name);
                    PlotActions.WriteXml(path, result);
                    WriteMsg($"Plot: Intermediate plot count={result.Count}; path={path}");
                }
                else
                {
                    WriteMsg($"Plot: Intermediate plot count={result.Count}");
                }
            }
            return r;
        }

        public bool TryGenerateGCode()
        {
            string path;
            if (StoreIntermediate)
            {
                path = BuildFilePath("{0}.p.xml", (object)Name);
                XmlStream.Write(path, TestPattern, create_backup: false, noexcept: false);
                WriteMsg($"Plot Gcode: created pattern; path=\"{path}\"");
            }

            if (!TryProcess(out List<PlotAction> plotactions)) return false;

            FilePath = BuildFilePath("{0}-{1}.nc", (object)Name, context.Now.ToString("yyyy-MM-ddTHH.mm.ss"));
            try
            {
                context.Open(FilePath);
                GenerateGCode(plotactions);
            }
            finally
            {
                context.Close();
            }
            WriteMsg($"Plot Gcode: created gcode; path=\"{FilePath}\"");
            return true;
        }

        void GenerateGCode(List<PlotAction> plotactions)
        {
            // prepare context
            context.X.Value = 0;
            context.Y.Value = 0;
            context.Index.Value = new GPoint() { X = 0, Y = 0 };

            var c = TestPattern.Count;
            context.IndexMax = new GPoint()
            {
                X = Math.Max(1, c.X - 1),
                Y = Math.Max(1, c.Y - 1),
            };

            context.F0.Value = Math.Min(Math.Max(
                Settings.App.Machine.G0FMinimum,
                TestPattern.SpeedG0Set),
                Settings.App.Machine.G0FMaximum);
            context.F0.Factor = 60.0;
            context.F.Factor = 60.0;

            context.S.GName = Settings.App.GCode.LaserPower;

            context.XLimits = TestPattern.XLimits;
            context.YLimits = TestPattern.YLimits;
            context.MachineLimits = new Axis()
            {
                ZHeight = new Scale<double>()
                {
                    Minimum = Settings.App.Machine.ZMinimum,
                    Maximum = Settings.App.Machine.ZMaximum
                },
                Speed = new Scale<double>()
                {
                    Minimum = Settings.App.Machine.G1FMinimum,
                    Maximum = Settings.App.Machine.G1FMaximum
                },
                Power = new Scale<int>()
                {
                    Minimum = Settings.App.Machine.PowerMinimum,
                    Maximum = Settings.App.Machine.PowerMaximum
                }
            };

            // intro
            Comment.WriteGcode(context, msg: $"This file is generated with {Settings.AppName}, v{Settings.AppVersion}, {context.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            if (context.GCode.TryGetConsumer(out string name)) Comment.WriteGcode(context, msg: $"GCode using {name} conventions or RS-274 interpretation");
            Comment.WriteGcode(context, msg: "Test and check the content before you use it. Use at your own risk, protect your eyes!");
            context.WriteLine();
            context.WriteLine(Settings.App.GCode.Intro);
            context.WriteLine();

            // add GCode opening comments (script settings)
            foreach (var p in plotactions)
            {
                if (p is not Comment) break;
                if (p.TryWriteGcode(context)) context.WriteLine();
            }
            context.WriteLine();

            PlotAction a;
            var f = context.GCode.FormatFloat;

            var msg = $"{Name} uses X from 0 to {TestPattern.Width.Ci(f)} mm and Y from 0 to {TestPattern.Height.Ci(f)} mm";
            UI.InfoFormat($"Plotter: {msg}");
            a = new Comment() { Message = msg };
            if (a.TryWriteGcode(context)) context.WriteLine();
            context.WriteLine();

            // add script GCode requirements
            a = new AddGcode() { GCode = Settings.App.GCode.AbsolutePostions, Info = new Comment() { Message = "absolute positions" } };
            if (a.TryWriteGcode(context)) context.WriteLine();
            a = new AddGcode() { GCode = Settings.App.GCode.LaserEnable, Info = new Comment() { Message = "laser enabled" } };
            if (a.TryWriteGcode(context)) context.WriteLine();

            // add GCode for test script.
            context.WriteLine(Settings.App.GCode.Header);
            context.WriteLine();

            context.WriteLaserIdle();
            context.Write($"G0");
            context.X.WriteGcode(context, forced: true);
            context.Y.WriteGcode(context, forced: true);
            context.F0.WriteGcode(context, forced: true);
            a = new Comment() { Message = "To outer pattern part #x=1, #y=1", Column = context.GCode.InfoColumn };
            if (a.TryWriteGcode(context)) context.WriteLine();

            bool comment_head = true;
            foreach (var p in plotactions)
            {
                if (comment_head)
                {
                    if (p is Comment) continue;
                    comment_head = false;
                }
                if (p.TryWriteGcode(context)) context.WriteLine();
            }
            context.WriteLaserIdle();
            a = new AddGcode() { GCode = Settings.App.GCode.LaserOff, Info = new Comment() { Message = "laser off" } };
            if (a.TryWriteGcode(context)) context.WriteLine();
            a = new AddGcode() { GCode = Settings.App.GCode.LaserDisable, Info = new Comment() { Message = "laser disabled" } };
            if (a.TryWriteGcode(context)) context.WriteLine();
            context.WriteLine(Settings.App.GCode.Footer);
        }

        void WriteMsg(string msg)
        {
            Log.InfoFormat(msg);
            if (context.Verbose) Loggers.UI.InfoFormat(msg);
        }

        static string BuildFileName(string format, params object[] args) => string.Format(format, args);

        static string BuildFilePath(string path, string format, params object[] args) => Path.Combine(path, BuildFileName(format, args));

        string BuildFilePath(string format, params object[] args) => BuildFilePath(BuildPath, BuildFileName(format, args));
        #endregion Actions
    }
}
