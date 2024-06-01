namespace Game
{
    public interface ISaveLoad
    {
        /// <summary>
        /// method to get instance json
        /// </summary>
        /// <returns>data to be saved</returns>
        public string Save();

        /// <summary>
        /// apply to load instance
        /// </summary>
        /// <param name="data">unique string returned by Save() method</param>
        public void Load(string data);
    }
}