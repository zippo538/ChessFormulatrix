using ChessAPI.Models;

namespace ChessAPI.Helpers;

public class TimerHelper
{
    private readonly TimerModel _timer;

    public TimeSpan RemainingTime 
        => _timer._remainingTime;
    public bool isFlagged 
        => _timer._remainingTime <= TimeSpan.Zero;

    public TimerHelper(TimerModel timer)
    {
        _timer = timer;
    }
    public void UpdateTime(TimeSpan elapsedTime)
    {
        if (isFlagged) 
            return;
        _timer._remainingTime -= elapsedTime;
        if(_timer._remainingTime <= TimeSpan.Zero) 
            _timer._remainingTime = TimeSpan.Zero;
    }

    public void ApplyIncrement()
    {
        if (!isFlagged) 
            _timer._remainingTime += _timer._increment;
    }
}