namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// Used to pick a forward direction for the Target Selector. 
    /// 
    /// In a platformer tipically the X axis (right) will be the forward direction.
    /// In a spaceship game for example, where you aim upwards, it's the Y axis.
    /// </summary>
    public enum ForwardDirection
    {
        X, Y, Minus_X, Minus_Y
    }
}
