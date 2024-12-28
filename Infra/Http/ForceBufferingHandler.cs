using System.Text;
namespace SentenceMining.Infra.Http
{
    public class ForceBufferingHandler : DelegatingHandler
    {
        public ForceBufferingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync(cancellationToken);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
