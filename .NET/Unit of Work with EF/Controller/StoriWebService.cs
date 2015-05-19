using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using STORI.Common.Models;
using STORI.Data.Common.Interfaces;
using STORI.Data.Interfaces;
using STORI.WebServices.Utils;

namespace STORI.WebServices.WebService
{

    public class StoriWebService<T> : IStoriWebService<T> where T : class, new()
    {

        private readonly IStoriUow _uow;
        private readonly IRepository<T> _repository;

        public StoriWebService(IStoriUow uow)
        {
            _uow = uow;
            _repository = uow.GetRepository<T>();
        }


        public ActionData GetAllData()
        {
            try
            {
                var data = _repository.GetAll().ToList();
                return new ActionData { Done = true, Data = data };
            }
            catch (Exception ex)
            {
                var msg = ex.GetOriginalException().Message;
                return new ActionData { Data = null, Error = new ActionErrorMessage { Message = msg } };
            }
        }

        public ActionData GetDataById(int id)
        {
            try
            {
                var data = new List<T> { _repository.GetById(id) };
                return new ActionData { Done = true, Data = data };
            }
            catch (Exception ex)
            {
                var msg = ex.GetOriginalException().Message;
                return new ActionData { Data = null, Error = new ActionErrorMessage { Message = msg } };
            }
        }

        public ActionData ServiceResult<TS>(IEnumerable<TS> sourceData, ServiceActionType serviceActionType)
            where TS : class
        {

            var keys = new List<int>();
            sourceData = sourceData.ToList();
            if (!sourceData.Any())
            {
                return new ActionData { Done = true, IEPKey = keys };
            }

            try
            {
                var properties = sourceData.First().GetType().GetProperties();

                foreach (var sourceObject in sourceData)
                {
                    var targetObject = new T();
                    var target = SimpleMatchCopy(sourceObject, targetObject, properties);
                    switch (serviceActionType)
                    {
                        case ServiceActionType.Insert:
                            _repository.Add(target);
                            break;
                        case ServiceActionType.Update:
                            _repository.Update(target);
                            break;
                        case ServiceActionType.Delete:
                            _repository.Delete(target);
                            break;
                    }
                    var iepKey = target.GetType().GetProperty("IEPKey");
                    if (iepKey == null) continue;

                    var key = iepKey.GetValue(target, null);
                    if (key != null)
                    {
                        keys.Add((int) key);
                    }
                }
                _uow.Commit();
                return new ActionData { Done = true, IEPKey = keys };
            }
            catch (Exception ex)
            {
                var msg = ex.GetOriginalException().Message;
                return new ActionData { Data = null, Error = new ActionErrorMessage { Message = msg } };
            }

        }

        private TD SimpleMatchCopy<TS, TD>(TS sourceObject, TD targetObject, PropertyInfo[] properties)
            where TS : class
            where TD : class
        {
            foreach (var sourceProp in properties)
            {
                var targetPro = targetObject.GetType().GetProperty(sourceProp.Name);
                if (targetPro == null || !targetPro.CanWrite)
                {
                    continue;
                }
                targetPro.SetValue(targetObject, sourceProp.GetValue(sourceObject, null), null);
            }
            return targetObject;
        }


    }
}
