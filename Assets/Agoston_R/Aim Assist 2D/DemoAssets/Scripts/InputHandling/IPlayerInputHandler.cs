namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.InputHandling
{
    public interface IPlayerInputHandler
    {
        bool Menu();

        bool Fire();

        float AimHorizontal();

        float AimVertical();

        float MoveHorizontal();

        float MoveVertical();

        bool Jump();
    }

}
