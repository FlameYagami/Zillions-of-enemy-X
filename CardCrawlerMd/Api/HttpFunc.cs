using System;

namespace CardCrawler.Api
{
    public class HttpFunc<T>
    {
        public static Func<HttpResult<T>, T> Selector = result =>
        {
            if (0 != result.resthead.errorCode)
                throw new Exception(result.resthead.message);
            return result.restbody;
        };
    }
}