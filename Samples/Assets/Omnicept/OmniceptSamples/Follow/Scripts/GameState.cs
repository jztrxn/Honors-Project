// (c) Copyright 2020 HP Development Company, L.P.

namespace HP.Glia.Examples.Follow
{
    [System.Serializable]
    public enum GameState{
        NotStarted,
        ShowingSequence,
        UserPlayingSequence,
        BetweenRounds
    }
}