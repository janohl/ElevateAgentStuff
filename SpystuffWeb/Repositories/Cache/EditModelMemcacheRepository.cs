using Common;
using Enyim.Caching.Memcached;
using Models;
using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Repositories.Cache
{
    /// <summary>
    /// EditModel repro used for locking objects for edit mode in memcache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditModelMemcacheRepository<T> : MemcacheRepository<EditModel<T>>
    {
        private const string EDIT_KEY = "&&EDIT&&_";

        protected override void LoadConfig(string configName)

        {
            base.LoadConfig(string.Format("EditModel{0}", configName));
        }
       
        protected string GetKey(Request request, T obj = default(T))
        {
            string key = base.GetKey(request);

            if (!key.Contains(EDIT_KEY))
            {
                key = string.Format("{0}{1}", EDIT_KEY, key);
            }

            return key;
        }

        private Request CreateEditRequest(Request request)
        {
            return new Request
            {
                Parameters = request.Parameters,
                Key = request.Key,
                LockBy = request.LockBy,
                LockForEdit = request.LockForEdit
            };
        }

        public new T Get(Request request)
        {
            var obj = default(T);

            if (request == null)
                throw new ArgumentNullException("request", "Parameter can't be null");

            if (request.LockForEdit)
            {
                EditModel<T> editObject;

                editObject = base.Get(CreateEditRequest(request));

                if (editObject != null)
                {
                    if (editObject.EditUser.Id == request.LockBy.Id)
                    {
                        obj = editObject.EditObject;
                    }
                    else
                    {
                        //TODO use better object names?
                        ThrowLockedBy(editObject.EditUser);
                    }
                }
                else
                {
                    //Only when negative ids.
                    try
                    {
                        Type t = request.Parameters.GetType();
                        PropertyInfo[] pi = t.GetProperties();
                        var prop = pi.FirstOrDefault(p => p.Name == "Id");

                        if (prop != null && ((int)(prop.GetValue(request.Parameters, null)) <= -1))
                        {

                            //Create default object. Uses user id as a way to get a unique way to save the object based on Id 
                            obj = default(T);
                            var propertyList = PropertyInfoHelper.GetProperties<T>();
                            propertyList.KeyProperty.SetValue(obj, request.LockBy.Id * -1);
                            editObject.EditObject = obj;
                            base.Save(editObject, CreateEditRequest(request));
                        }
                    }
                    catch (Exception exp)
                    {
                        Logger.Error("Trying to find id when getting default object.", exp);
                    }

                }

            }

            return obj;
        }


        public void Save(T obj, Request request)
        {
            if (request != null)
            {
                if (request.LockBy != null)
                {
                    EditModel<T> editObject;

                    Request editRequest = CreateEditRequest(request);
                    PropertyList propertyList;

                    /*
                    //For new unsaved objects and more than one user has unsaved objects in memory
                    propertyList =  PropertyInfoHelper.GetProperties<T>();
                    int id = propertyList.KeyProperty.GetValue(obj, null);
                    if (id < 0)
                    {
                        id = request.LockBy.Id * -1;   
                    }*/


                    if (editRequest.Parameters == null)
                    {
                        //Create parameters based on Key attribute on Object						
                        propertyList = PropertyInfoHelper.GetProperties<T>();

                        editRequest.Parameters = new { Id = propertyList.KeyProperty.GetValue(obj, null) };
                    }


                    editObject = base.Get(editRequest);

                    if (request.LockForEdit)
                    {
                        if (editObject == null)
                        {
                            //Create a custom user for locked by to avoid having to much data in memcache. Only need id and name. Use dunamic is an option.
                            var lockUser = new EditUser { Id = request.LockBy.Id, Name = request.LockBy.Name };

                            editObject = new EditModel<T> { EditObject = obj, EditUser = lockUser };
                        }
                        else
                        {
                            editObject.EditObject = obj;
                        }

                        if (editObject != null)
                            base.Save(editObject, editRequest);
                    }
                    else
                    {
                        if (editObject != null)
                        {
                            if (editObject.EditUser.Id == request.LockBy.Id)
                                Delete(editRequest);
                            else
                                ThrowLockedBy(editObject.EditUser);
                        }
                        else
                        {
                            //We guess that we created a new item and that item has a negative id of user.id *-1                            
                            //checked if this item exists
                            editRequest.Key = null;
                            editRequest.Parameters = new { Id = request.LockBy.Id * -1 };
                            editObject = base.Get(editRequest);
                            if (editObject != null)
                            {
                                if (editObject.EditUser.Id == request.LockBy.Id)
                                    Delete(editRequest);
                                else
                                    ThrowLockedBy(editObject.EditUser);
                            }
                        }
                    }
                }
            }

        }

        private void ThrowLockedBy(EditUser user)
        {
            throw new ObjectLockedException(string.Format("{0} locked by {1}", typeof(T).Name, user.Name), user.Name);
        }


        public new IEnumerable<T> GetList(Request request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "Parameter can't be null");

            IEnumerable<T> list = null;

            if (request.LockForEdit)
            {
                EditModel<IEnumerable<T>> editObject;
                
                //Special for just editmodel with list parameter
                var key = GetKey(CreateEditRequest(request));
                editObject = _client.Get<EditModel<IEnumerable<T>>>(key);

                if (editObject != null)
                {
                    if (editObject.EditUser.Id == request.LockBy.Id)
                    {
                        list = editObject.EditObject;
                    }
                    else
                    {
                        ThrowLockedBy(editObject.EditUser);
                    }
                }

            }
            return list;
        }
        
        public void SaveList(IEnumerable<T> list, Request request = null)
        {
            if (request != null)
            {
                if (request.LockBy != null)
                {
                    Request editRequest = CreateEditRequest(request);

                    EditModel<IEnumerable<T>> editObject;
                    //Special for just editmodel with list parameter
                    var key = GetKey(editRequest);
                    editObject = _client.Get<EditModel<IEnumerable<T>>>(key);

                    if (request.LockForEdit)
                    {
                        if (editObject == null)
                        {
                            //Create a custom user for locked by to avoid having to much data in memcache. Only need id and name. Use dunamic is an option.
                            var lockUser = new EditUser { Id = request.LockBy.Id, Name = request.LockBy.Name };

                            editObject = new EditModel<IEnumerable<T>> { EditObject = list, EditUser = lockUser };
                        }
                        else
                        {
                            editObject.EditObject = list;
                        }
                        var storeResult = _client.Store(StoreMode.Set, key, editObject, GetTTL());

                    }
                    else
                    {
                        if (editObject != null)
                        {
                            if (editObject.EditUser.Id == request.LockBy.Id)
                                Delete(request);
                            else
                                ThrowLockedBy(editObject.EditUser);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// no need to do anything. Id will not be reused due to identity in database. It will self expire in cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public override int Delete(Request request = null)
        {
            var editRequest = CreateEditRequest(request);
            EditModel<T> editObject;

            editObject = base.Get(editRequest);

            if (editObject != null)
                if (editRequest.LockBy.Id == request.LockBy.Id)
                    return base.Delete(editRequest);

            return 0;
        }
    }




	[Serializable] 
	public class EditModel<T>
	{
		public EditModel()
		{
		}
		
		public T EditObject {get;set;}
		public EditUser EditUser { get; set; }

	}

    [Serializable]
	public class EditUser
	{
		public int Id { get; set; }
		public string Name { get; set;}
	}
}
