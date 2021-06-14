namespace Pacman
{
    public interface IPausable
    {
        bool IsPaused { get; }
        
        void Pause();
        void Resume();
        void Stop();
        void EnableVisual();
        void DisableVisual();
        void BlinkVisual();

    }
}