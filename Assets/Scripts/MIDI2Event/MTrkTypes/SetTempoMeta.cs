namespace MIDI2EventSystem
{
    internal class SetTempoMeta : MTrkEvent
    {
        //microseconds per quarter
        public uint USPerQuarter { get; set; }

        public SetTempoMeta(uint delta, uint USPerQuarter)
            : base(delta)
        {
            this.USPerQuarter = USPerQuarter;
        }
    }
}
