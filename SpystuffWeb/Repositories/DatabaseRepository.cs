using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Reflection;
using Repositories.Config;
using Models;
using Models.Common;
using Common;

namespace Repositories
{

    public class DatabaseRepository<T> : BaseRepository<DBConfig>, IDataRepository<T>  where T : new()
    {
        /// <summary>
        /// Create database connection
        /// </summary>
        /// <returns></returns>
        protected SqlConnection CreateConnection(string connectionName = "AgentStuff")
        {
            return new SqlConnection(_config.ConnectionStrings[connectionName].ConnectionString);
        }

        /// <summary>
        /// Get data for a specifig type
        /// Node default command is [namespace].p_Get[Class name]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual T Get(Request request)
        {
            return Get(request, false);
        }

        /// <summary>
        /// Get data for a specifig type
        /// Node default command is [namespace].p_Get[Class name]</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="obj"></param>
        /// <param name="multiple">Returns a multiresult set, override SetData to handle"/></param>
        /// <returns></returns>
        protected T Get(Request request, bool multiple)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                T obj;
                if (multiple)
                {
                    // ReSharper disable RedundantArgumentName
                    var multi = conn.QueryMultiple(GetCommand(), param: request.Parameters, commandType: CommandType.StoredProcedure);
                    // ReSharper restore RedundantArgumentName
                    obj = SetData(multi);
                }
                else
                {
                    obj = conn.Query<T>(GetCommand(), request.Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }

                return obj;
            }
        }

        /// <summary>
        /// Set data. Override to to handle multipple resultsets. Takes only first result as Default 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual T SetData(SqlMapper.GridReader result)
        {
            return result.Read<T>().FirstOrDefault();
        }


        /// <summary>
        /// Get List data. command [namespace].p_Get[Class name]List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(Request request = null)
        {
            return GetList(request, false);
        }

        /// <summary>
        /// Get List data. command [namespace].p_Get[Class name]List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="multiple"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetList(Request request, bool multiple)
        {
            object parameters = null;

            if (request != null)
            {
                parameters = request.Parameters;
            }

            using (var conn = CreateConnection())
            {
                conn.Open();
                if (multiple)
                {
                    SqlMapper.GridReader multi = conn.QueryMultiple(GetListCommand(), parameters, commandType: CommandType.StoredProcedure);
                    return SetListData(multi);
                }
                return conn.Query<T>(GetListCommand(), parameters, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Set data for List result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> SetListData(SqlMapper.GridReader result)
        {
            return result.Read<T>();
        }


        /// <summary>
        /// Save object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="request"></param>
        public virtual void Save(T obj, Request request = null)
        {
            using (var conn = CreateConnection())
            {
                //Open connection
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    SaveObject(obj, request, conn, transaction);

                    transaction.Commit();
                    RemoveDeletedObject(obj);
                }
                catch (Exception exp)
                {
                    Logger.Error(exp);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected virtual void RemoveDeletedObject(T obj)
        {
            var parentProperties = PropertyInfoHelper.GetProperties(obj.GetType());

            foreach (PropertyInfo dataListProp in parentProperties.DataListProperties)
            {
                //Child object on parent that is a List
                dynamic list = dataListProp.GetValue(obj, null);

                var tmoList = new List<IData>();
                tmoList.AddRange(list);
                foreach (var listObj in tmoList.GetDeleted())
                {
                    int count = list.Count;
                    for (var ix = 0; ix < count; )
                    {
                        if (listObj == list[ix])
                        {
                            list.Remove(list[ix]);
                            count--;
                        }
                        else
                            ix++;
                    }
                }

            }

        }

        public void SaveList(IEnumerable<T> list, Request request = null)
        {
            using (var conn = CreateConnection())
            {
                //Open connection
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    SaveCommonListObjects(list, request, conn, transaction);

                    foreach (var obj in list)
                        SaveObject(obj, request, conn, transaction);

                    transaction.Commit();
                }
                catch
                {
                    //loggning?
                    transaction.Rollback();
                    throw;

                }
            }

        }
        /// <summary>
        /// Save common data for all items in a List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="request"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public virtual void SaveCommonListObjects(IEnumerable<T> list, Request request, IDbConnection connection, IDbTransaction transaction)
        {

        }
        protected virtual void SaveObject(T obj, Request request, IDbConnection connection, IDbTransaction transaction)
        {
            //Create dummy object whenever request's null 
            string command = null;
            Object parameters = null;


            if (request != null)
            {
                parameters = request.Parameters;
            }

            //Check if object has a Key. Then use that as the 
            PropertyList properties = PropertyInfoHelper.GetProperties(obj);

            //Create a new set of dynamic parameters
            DynamicParameters dynParam = CreateDynParameters(obj, parameters);

            //Execute
           
            connection.Execute(SaveCommand(command), dynParam, commandType: CommandType.StoredProcedure, transaction: transaction);

            if (properties.KeyProperty != null)
            {
                //Set the value
                properties.KeyProperty.SetValue(obj, dynParam.Get<object>(properties.KeyProperty.Name), null);
            }

            SaveSubData(obj, request, connection, transaction);
        }



        protected DynamicParameters CreateDynParameters(object obj, Object parameters)
        {
            DynamicParameters dynParam;
            PropertyList properties = PropertyInfoHelper.GetProperties(obj);
            //Custom parameters
            if (parameters != null)
            {
                dynParam = new DynamicParameters(parameters);
            }
            else if (properties.Any() && properties.KeyProperty != null)
            {
                dynParam = new DynamicParameters();
                foreach (PropertyInfo dataProp in properties.DataProperties)
                {
                    dynParam.Add(dataProp.Name, dataProp.GetValue(obj, null));
                }

                foreach (PropertyInfo dataObjProp in properties.DataObjectProperties)
                {

                    dynamic dataObj = dataObjProp.GetValue(obj, null);
                    if (dataObj != null)
                    {
                        PropertyList propObjproperties = PropertyInfoHelper.GetProperties(dataObj);

                        if (propObjproperties.KeyProperty != null)
                        {
                            dynParam.Add(string.Format("{0}{1}", propObjproperties.KeyProperty.ReflectedType.Name, propObjproperties.KeyProperty.Name), propObjproperties.KeyProperty.GetValue(dataObj, null));
                        }
                    }
                }

            }
            else
            {
                //Fallback, use all properties from the object as parameters.
                dynParam = new DynamicParameters(obj);
            }

            //Get key field on object

            if (properties.KeyProperty != null)
            {
                //Looked in Dapper source to see that it overwrites existing parameters =) 
                dynParam.Add(properties.KeyProperty.Name, properties.KeyProperty.GetValue(obj, null), direction: ParameterDirection.InputOutput);
            }

            return dynParam;
        }

        /// <summary>
        /// Delete object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public int Delete(Request request)
        {
            //Follow the pattern
            using (var conn = CreateConnection())
            {
                conn.Open();
                
                return conn.Execute(DeleteCommand(), request.Parameters, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Get command on format [namespace].p_Get[Class name]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual string GetCommand()
        {
            var type = typeof(T);

            // ReSharper disable PossibleNullReferenceException
            var schema = type.Namespace.Split('.').Last().ToLower();
            // ReSharper restore PossibleNullReferenceException

            return string.Format("{0}.p_Get{1}", schema, type.Name);

        }

        /// <summary>
        /// Get command on format [namespace].p_Get[Class name]List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual string GetListCommand()
        {
            var type = typeof(T);
            // ReSharper disable PossibleNullReferenceException
            var schema = type.Namespace.Split('.').Last().ToLower();
            // ReSharper restore PossibleNullReferenceException

            return string.Format("{0}.p_Get{1}List", schema, type.Name);
        }

        protected virtual string SaveCommand()
        {
            return SaveCommand(null);
        }

        /// <summary>
        /// Get command for save on format [namespace].p_Save[Class name]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual string SaveCommand(string command)
        {
            if (command != null)
                return command;

            return GetSaveCommandFromType(typeof(T));
        }

        protected string GetSaveCommandFromType(Type type)
        {
            // ReSharper disable PossibleNullReferenceException
            var schema = type.Namespace.Split('.').Last().ToLower();
            // ReSharper restore PossibleNullReferenceException

            return string.Format("{0}.p_Save{1}", schema, type.Name);
        }


        /// <summary>
        /// Get command for save on format [namespace].p_Delete[Class name]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual string DeleteCommand(string command = null)
        {
            if (command != null)
                return command;

            return GetDeleteCommandFromType(typeof(T));
        }

        protected string GetDeleteCommandFromType(Type type)
        {
            // ReSharper disable PossibleNullReferenceException
            var schema = type.Namespace.Split('.').Last().ToLower();
            // ReSharper restore PossibleNullReferenceException

            return string.Format("{0}.p_Delete{1}", schema, type.Name);
        }

        /// <summary>
        /// Save aditional data availabel on the object. Looks for DataObjAttribute  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="request"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        protected virtual void SaveSubData(T obj, Request request, IDbConnection connection, IDbTransaction transaction)
        {
            var parentProperties = PropertyInfoHelper.GetProperties(obj.GetType());

            foreach (PropertyInfo dataListProp in parentProperties.DataListProperties)
            {
                //Child object on parent that is a List
                dynamic list = dataListProp.GetValue(obj, null);

                var tmoList = new List<IData>();
                tmoList.AddRange(list);

                foreach (var listObj in tmoList.GetDeleted())
                {
                    var listObjSqlParameters = new DynamicParameters();

                    //Add the parent key id format [ClassName] + [Key property name]										
                    listObjSqlParameters.Add(parentProperties.KeyProperty.Name, parentProperties.KeyProperty.GetValue(listObj, null));
                    string deleteSp = GetDeleteCommandFromType(listObj.GetType());
                    
                    connection.Execute(deleteSp, listObjSqlParameters, transaction, commandType: CommandType.StoredProcedure);
                }

                foreach (var listObj in tmoList.GetNonDeleted())
                {
                    //Get props for an ovj in the List
                    PropertyList listObjProperties = PropertyInfoHelper.GetProperties(listObj);

                    DynamicParameters listObjSqlParameters = CreateDynParameters(listObj, null);

                    //Add the parent key id format [ClassName] + [Key property name]										
                    listObjSqlParameters.Add(parentProperties.KeyProperty.ReflectedType.Name + parentProperties.KeyProperty.Name, parentProperties.KeyProperty.GetValue(obj, null));
                    string saveSp = GetSaveCommandFromType(listObj.GetType());

                    connection.Execute(saveSp, listObjSqlParameters, transaction, commandType: CommandType.StoredProcedure);

                    if (listObjProperties.KeyProperty != null)
                    {
                        //Set the value
                        listObjProperties.KeyProperty.SetValue(listObj, listObjSqlParameters.Get<object>(listObjProperties.KeyProperty.Name), null);
                    }

                }
            }


        }
    }
}
