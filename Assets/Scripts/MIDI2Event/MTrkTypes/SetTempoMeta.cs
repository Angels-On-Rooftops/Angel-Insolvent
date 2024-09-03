namespace midi2event
{
    internal class SetTempoMeta : MTrkEvent
    {
        public uint USPerQuarter { get; set; }

        public SetTempoMeta(uint delta, uint MSPerQuarter)
            : base(delta)
        {
            this.USPerQuarter = MSPerQuarter;
        }
    }
}
