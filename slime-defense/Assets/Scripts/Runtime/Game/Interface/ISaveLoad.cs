namespace Game
{
    public interface ISaveLoad
    {
        public string Save();
        public void Load(string data);
    }
}