namespace MIDI2EventSystem
{
    internal class EndTrackMeta : MTrkEvent
    {
        public EndTrackMeta(uint delta)
            : base(delta)
        {
            this.Delta = delta;
        }
    }
}
