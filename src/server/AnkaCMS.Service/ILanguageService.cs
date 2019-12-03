using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.ValueObjects;
using System.Collections.Generic;

namespace AnkaCMS.Service
{
    public interface ILanguageService : ICrudService<LanguageModel>
    {
        List<IdCodeName> List();
    }
}
