using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.Common
{
    public static class BaseListExtension
    {
        public static IEnumerator<T> GetEnumerator<T>(this List<T> list) where T : IData
        {
            IEnumerator<T> ie = list.GetEnumerator();
            while (ie.MoveNext())
            {
                if (ie.Current.State != ObjectState.Deleted)
                    yield return ie.Current;
            }
        }

        public static List<T> GetDeleted<T>(this List<T> list) where T : IData
        {
            return list.Where(i => i.State == ObjectState.Deleted).ToList<T>();
        }

        public static List<T> GetNonDeleted<T>(this List<T> list) where T : IData
        {
            return list.Where(i => i.State != ObjectState.Deleted).ToList<T>();
        }

        public static void Add<T>(List<T> list, T item) where T : IData
        {
            if (item.Id == 0)
            {
                item.Id = -1;
                item.State = ObjectState.New;
            }
            while (list.Exists(i => i.Id == item.Id))
                item.Id--;

            list.Add(item);
        }

        public static T Find<T>(this List<T> list, int id) where T : IData
        {
            return list.Find(i => i.Id == id);
        }

        public static void Delete<T>(this List<T> list, T item) where T : IData
        {
            if (item != null)
                item.State = ObjectState.Deleted;
        }

        public static void Delete<T>(this List<T> list, int id) where T : IData
        {
            T item;
            item = list.Find(i => i.Id == id);
            if (item != null)
                list.Delete<T>(item);
        }

    }

    [Serializable]
    public class BaseList<T> : List<T> where T : IData
    {
        public new IEnumerator<T> GetEnumerator()
        {
            IEnumerator<T> ie = base.GetEnumerator();
            while (ie.MoveNext())
            {
                if (ie.Current.State != ObjectState.Deleted)
                    yield return ie.Current;
            }
        }

        public new void ForEach(Action<T> action)
        {
            base.ForEach(action);
        }

        public new void Add(T item)
        {
            if (item.Id == 0)
            {
                item.Id = -1;
                item.State = ObjectState.New;
            }
            while (this.Exists(i => i.Id == item.Id))
                item.Id--;

            base.Add(item);
        }

        public List<T> GetDeleted()
        {
            return this.Where(i => i.State == ObjectState.Deleted).ToList<T>();
        }

        public List<T> GetNonDeleted()
        {
            return this.Where(i => i.State != ObjectState.Deleted).ToList<T>();
        }
        /// <summary>
        /// Set item to status Deleted.
        /// </summary>
        /// <param name="item"></param>
        public void Delete(T item)
        {
            if (item != null)
                item.State = ObjectState.Deleted;
        }

        public T Find(int id)
        {
            return this.Find(i => i.Id == id);
        }

        public void Delete(int id)
        {
            T item;
            item = this.Find(i => i.Id == id);
            if (item != null)
                this.Delete(item);
        }
    }
}
