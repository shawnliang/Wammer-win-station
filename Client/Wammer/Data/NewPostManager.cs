
using System.Collections.Generic;

namespace Waveface
{
    public class NewPostManager
    {
        public List<NewPostItem> Items { get; set; }

        public NewPostManager()
        {
            Items = new List<NewPostItem>();
        }

        public void Add(NewPostItem item)
        {
            Items.Add(item);
        }

        public void Remove(NewPostItem item)
        {
            Items.Remove(item);
        }
    }
}
