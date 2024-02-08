using Terraria.GameContent.Animations;

namespace ImproveGame.Common.AnimationActions;

/// <summary>
/// 跳跃，目前只支持向上跳然后下落到更下面
/// </summary>
public class NPCJump : Actions.NPCs.INPCAction
{
    private Vector2 _jumpSpeed;
    private float _groundOffset;
    private float _acceleration;
    private float _delay;
    private float _initialPositionY;
    private Vector2 _initialPosition;

    public int ExpectedLengthOfActionInFrames => CalculateTime();

    public NPCJump(Vector2 jumpSpeed, float groundOffset, float acceleration, out int dedicatedTime)
    {
        _jumpSpeed = jumpSpeed;
        _groundOffset = groundOffset;
        _acceleration = acceleration;
        dedicatedTime = CalculateTime();
    }

    private int CalculateTime()
    {
        // v = at => t = v/a
        // 回到初始点的位置
        int timeBackToInitial = (int)(Math.Abs(_jumpSpeed.Y / _acceleration)) * 2;
        // 从初始点到地面的位置
        // y = v0t + 1/2at^2 => t = (-sqrt(2ay + v0^2) - v0) / a
        int timeToGround = Math.Abs((int)(((float)-Math.Sqrt(2f * _groundOffset * _acceleration + _jumpSpeed.Y * _jumpSpeed.Y) - _jumpSpeed.Y) / _acceleration));

        return timeToGround + timeBackToInitial;
    }

    public void BindTo(NPC obj)
    {
    }

    public void SetDelay(float delay)
    {
        _delay = delay;
    }

    public void ApplyTo(NPC obj, float localTimeForObj)
    {
        if (localTimeForObj < _delay)
            return;
        if (localTimeForObj == _delay)
            _initialPosition = obj.position;

        int dedicatedTime = CalculateTime();
        float time = localTimeForObj - _delay;
        if (time >= dedicatedTime) // 不这么写会Bug，因为dedicatedTime之后这个也在执行
        {
            obj.position.Y = _groundOffset;
            obj.position.X = _jumpSpeed.X * dedicatedTime;
            obj.position += _initialPosition;
            return;
        }

        // y = v0t + 1/2at^2
        obj.position.Y = _jumpSpeed.Y * time + 0.5f * _acceleration * time * time;
        obj.position.X = _jumpSpeed.X * time;
        obj.position += _initialPosition;
        // v = v0 + at
        obj.velocity.Y = _jumpSpeed.Y + _acceleration * time;
        obj.velocity.X = _jumpSpeed.X;

        obj.direction = (obj.spriteDirection = ((obj.velocity.X > 0f) ? 1 : (-1)));
    }
}