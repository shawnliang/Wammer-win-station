#region

using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Spring.Http;
using Spring.Http.Converters;
using Spring.Http.Converters.Json;
using Spring.Rest.Client;

#endregion

namespace Spring.RestQuickStart
{
    public partial class GoogleImagesForm : Form
    {
        public GoogleImagesForm()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            ResultsFlowLayoutPanel.Controls.Clear();

            IHttpMessageConverter njsonConverter = new NJsonHttpMessageConverter();
            njsonConverter.SupportedMediaTypes.Add(new MediaType("text", "javascript"));

            RestTemplate template = new RestTemplate();
            template.MessageConverters.Add(njsonConverter);

            template.GetForObjectAsync("https://ajax.googleapis.com/ajax/services/search/images?v=1.0&rsz=8&q={query}",
                                       delegate(RestOperationCompletedEventArgs<JToken> r)
                                           {
                                               if (r.Error == null)
                                               {
                                                   foreach (JToken _jToken in r.Response.Value<JToken>("responseData").Value<JArray>("results"))
                                                   {
                                                       PictureBox _pBox = new PictureBox();
                                                       _pBox.ImageLocation = _jToken.Value<string>("tbUrl");
                                                       _pBox.Height = _jToken.Value<int>("tbHeight");
                                                       _pBox.Width = _jToken.Value<int>("tbWidth");

                                                       ToolTip _tt = new ToolTip();
                                                       _tt.SetToolTip(_pBox, _jToken.Value<string>("visibleUrl"));

                                                       ResultsFlowLayoutPanel.Controls.Add(_pBox);
                                                   }
                                               }
                                           }, SearchTextBox.Text);

            /*
            template.GetForObjectAsync<GImagesResponse>("https://ajax.googleapis.com/ajax/services/search/images?v=1.0&rsz=8&q={query}",
                delegate(RestOperationCompletedEventArgs<GImagesResponse> r)
                {
                    if (r.Error == null)
                    {
                        foreach (GImage gImage in r.Response.Data.Items)
                        {
                            PictureBox pBox = new PictureBox();
                            pBox.ImageLocation = gImage.ThumbnailUrl;
                            pBox.Height = gImage.ThumbnailHeight;
                            pBox.Width = gImage.ThumbnailWidth;

                            ToolTip tt = new ToolTip();
                            tt.SetToolTip(pBox, gImage.SiteUrl);

                            this.ResultsFlowLayoutPanel.Controls.Add(pBox);
                        }
                    }
                }, this.SearchTextBox.Text);
            */
        }

        /*
        [JsonObject]
        public class GImagesResponse
        {
            private GImagesResults _data;
            private int _status;

            [JsonProperty(PropertyName = "responseData")]
            public GImagesResults Data 
            {
                get { return _data; }
                set { _data = value; }
            }

            [JsonProperty(PropertyName = "responseStatus")]
            public int Status 
            {
                get { return _status; }
                set { _status = value; }
            }
        }

        [JsonObject]
        public class GImagesResults
        {
            private List<GImage> _items;

            [JsonProperty(PropertyName = "results")]
            public List<GImage> Items
            {
                get { return _items; }
                set { _items = value; }
            }
        }

        [JsonObject]
        public class GImage
        {
            private string _siteUrl;
            private int _thumbnailWidth;
            private int _thumbnailHeight;
            private string _thumbnailUrl;


            [JsonProperty(PropertyName = "visibleUrl")]
            public string SiteUrl
            {
                get { return _siteUrl; }
                set { _siteUrl = value; }
            }

            [JsonProperty(PropertyName = "tbWidth")]
            public int ThumbnailWidth
            {
                get { return _thumbnailWidth; }
                set { _thumbnailWidth = value; }
            }

            [JsonProperty(PropertyName = "tbHeight")]
            public int ThumbnailHeight
            {
                get { return _thumbnailHeight; }
                set { _thumbnailHeight = value; }
            }

            [JsonProperty(PropertyName = "tbUrl")]
            public string ThumbnailUrl
            {
                get { return _thumbnailUrl; }
                set { _thumbnailUrl = value; }
            }
        }
        */
    }
}