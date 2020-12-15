using OccultFriend.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IEmailTemplate
    {
        Task<string> GenerateTemplateDrawEmail(string template, object viewModel);

        string GenerateTextNamesDuplicate(HashSet<string> names);

    }
}
