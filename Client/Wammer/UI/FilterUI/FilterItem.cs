namespace Waveface.FilterUI
{
    public class FilterItem
    {
        public string Name { get; set; }
        public string Filter { get; set; }

        public string searchfilter_id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
