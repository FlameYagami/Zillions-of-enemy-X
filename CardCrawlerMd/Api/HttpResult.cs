namespace CardCrawler.Api
{
    public class HttpResult<T>
    {
        public RestHead resthead { get; set; }
        public T restbody { get; set; }
        public Extend extend { get; set; }

        public class RestHead
        {
            public int errorCode { get; set; }
            public string message { get; set; }
        }

        public class Extend
        {
            public int totalNum { get; set; }
            public int rowum { get; set; }
        }
    }
}