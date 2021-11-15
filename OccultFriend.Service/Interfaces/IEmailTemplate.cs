using System.Collections.Generic;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IEmailTemplate
    {
        Task<string> GenerateTemplateDrawEmail(string template, object viewModel);

        string GenerateTemplateTable(Dictionary<string, string> dicFriendDuplicate);

    }
}
