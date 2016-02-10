using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace CreativeColon.Raven.Identity.Domain
{
    public class SmsService : IIdentityMessageService
    {
        public virtual async Task SendAsync(IdentityMessage message)
        {
            await Task.FromResult(0);
        }
    }
}
