using System;
using Refit;

namespace WebCrawler.Api
{
    public class RequestApi
    {
        private RequestApi(string url)
        {
            RequestService = RestService.For<IRequestService>(BaseUrl);
        }

        private static string BaseUrl { get; set; }

        private static RequestApi _requestApi { get; set; }

        private IRequestService RequestService { get; }

        private static RequestApi Instance()
        {
            if (null != BaseUrl) return _requestApi ?? new RequestApi(BaseUrl);
            BaseUrl = $"https://www.zxtcg.com";
            return _requestApi ?? new RequestApi(BaseUrl);
        }

        /// <summary>
        ///     登录
        /// </summary>
        public static IObservable<object> GetCardList()
        {
            return Instance().RequestService.GetCardList(1, 1, 1, 1, "E09　EX サマ・ドラ");
        }
    }
}