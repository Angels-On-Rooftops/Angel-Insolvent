public class ControlConstants
{
    // all objects on layers 2 and 3 are ignored by character and camera raycasts
    public const int RAYCAST_MASK = ~(1 << 2 | 1 << 3);

    public const int CLIMBABLE_WALL_RAYCAST_MASK = 1 << 6;
}
