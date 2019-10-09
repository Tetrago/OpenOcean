public static class WorldRand
{
    private static System.Random random_;

    public static System.Random Random
    {
        get
        {
            if(random_ == null)
                random_ = new System.Random(World.instance_.seed_);
            return random_;
        }
    }
}
