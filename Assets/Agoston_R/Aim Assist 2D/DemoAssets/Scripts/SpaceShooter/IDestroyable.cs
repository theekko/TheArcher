using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public interface IDestroyable
    {
        Color Color { get; }
        Vector3 Position { get; }
        GameObject GameObject { get; }
    }

}
