namespace Santutu.Core.Base.Runtime.Enums
{
    public enum ActiveState
    {
        None = 0,
        Active = 1,
        Deactive = 2,
    }

    public static class ExtendActiveState
    {
        public static bool ToBool(this ActiveState activeState)
        {
            if (activeState == ActiveState.Active)
            {
                return true;
            }

            return false;
        }
    }
}