using System.Collections.Generic;
using STORI.Common.Models;

namespace STORI.WebServices.WebService
{
    public interface IStoriWebService<T> where T : class, new()
    {
        ActionData GetAllData();
        ActionData GetDataById(int id);
        ActionData ServiceResult<TS>(IEnumerable<TS> sourceData, ServiceActionType serviceActionType)
            where TS : class;
    }
}