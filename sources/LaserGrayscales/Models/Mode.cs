using As.Applications.Data;

namespace As.Applications.Models
{
    public enum Mode
    {
        Power = 1,
        Speed
    }

    public static class ModeExtensions
    {
        public static int Minimum(this Mode me)
        {
            return me switch
            {
                Mode.Power => Config.AppConfig?.PowerMinimum ?? 0,
                Mode.Speed => Config.AppConfig?.SpeedMinimum ?? 1,
                _ => 0,
            };
        }
        public static int Maximum(this Mode me)
        {
            return me switch
            {
                Mode.Power => Config.AppConfig?.PowerMaximum ?? 255,
                Mode.Speed => Config.AppConfig?.SpeedMaximum ?? 70,
                _ => 100,
            };
        }

        public static Mode Covariant(this Mode me)
            => (me == Mode.Power) ? Mode.Speed : Mode.Power;
    }
}
