public partial class Slime
{
    public class Builder
    {
        //services
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        private string key;
        private int lv;

        public Builder()
        {
            
        }

        public Builder Key(string key)
        {
            this.key = key;
            return this;
        }

        public Builder Lv(int lv)
        {
            this.lv = lv;
            return this;
        }

        public Slime Build()
        {
            var slime = Instantiate(resourceLoader.slimeModels[key]);
            slime.lv.Value = lv;
            return slime;
        }
    }
}