namespace AurumGames.Animation.Tracks
{
    public sealed class VoidTrack : ITrack
    {
        public float FullDuration { get; }

        public VoidTrack()
        {
            
        }

        public VoidTrack(int duration)
        {
            FullDuration = duration;
        }
         
        public void Evaluate(float time, bool isBackwards)
        {
            
        }

        public void ResetState()
        {
            
        }
    }
}