public interface IScoreKeeper
{
    public void AddScore(int score);
    public void RemoveScore(int score);
    public int GetScore();
    public void SetScore(int score);
}