public partial class Slime
{
    public class Builder
    {
        //services
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        private Slime slime;
        private int lv;

        public Builder()
        {
            
        }

        public Builder Lv(int lv)
        {
            this.lv = lv;
            return this;
        }

        public Slime Build()
        {
            return slime;
        }
    }
}