
using ChessAPI.Helpers;
using ChessAPI.Models;
using ChessAPI.Models.Enums;
using Spectre.Console;
using Timer = System.Timers.Timer;

namespace ChessAPI.Services;

public class TimerService
{
    private readonly TimerHelper _whiteTimer;
    private readonly TimerHelper _blackTimer;

    private PieceColor _currentTurn;
    private Timer? _ticker;

    private DateTime _lasTick;
    private bool _isRunning;
    public event Action<PieceColor>? TimeExpired;

    public TimerService(TimeSpan initialTime)
    {
        _whiteTimer = new TimerHelper(
            new TimerModel(initialTime, PieceColor.White)
            );
        _blackTimer = new TimerHelper(
            new TimerModel(initialTime,PieceColor.Black)
        );
        _currentTurn = PieceColor.White;
    }

    public TimeSpan WhiteTime => _whiteTimer.RemainingTime;
    public TimeSpan BlackTime => _blackTimer.RemainingTime;
    public PieceColor CurrentTurn => _currentTurn;
    public bool IsRunning => _isRunning;

    public void Start()
    {
        if(_isRunning) 
            return;
        _isRunning = true;
        _lasTick = DateTime.UtcNow;
        _ticker = new Timer(100);
        _ticker.Elapsed += Tick;
        _ticker.AutoReset = true;
        _ticker.Start();

    }

    public void Pause()
    {
        if (!_isRunning)
            return;
        _isRunning = false;
        _ticker?.Stop();
        _ticker?.Dispose();
        _ticker = null;
    }
    
    private void Tick(object? sender, EventArgs e)
    {
        // Tick Tiap 100 ms
        if (!_isRunning)
            return;
        var now = DateTime.UtcNow;
        var elapsed = now - _lasTick;
        _lasTick = now;
        var activeTimer = GetCurrentTimer();
        
        activeTimer.UpdateTime(elapsed);

        if (activeTimer.isFlagged)
        {
            Pause();
            TimeExpired?.Invoke(_currentTurn);
        }
    }

    public void SwitchTurn()
    {
        if(!_isRunning)
            return;
        // GetCurrentTimer().ApplyIncrement();
        _currentTurn = _currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
        _lasTick = DateTime.UtcNow;
    }

    private TimerHelper GetCurrentTimer()
    {
        return _currentTurn == PieceColor.White  ? _whiteTimer : _blackTimer;
    }

}