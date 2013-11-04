using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Common
{
    [Serializable]
    public class BaseDataClass : IData
    {
        [Required]
        [KeyAttribute]
        public int Id { get; set; }

        public BaseDataClass()
        {
            this.Id = -1;
        }

        public bool IsDeleted()
        {
            return (this.State == ObjectState.Deleted);
        }

        public bool IsUpdated()
        {
            return (this.State == ObjectState.Updated);
        }

        public bool IsNew()
        {
            return this.Id < 0;
        }

        public void Delete()
        {
            this.State = ObjectState.Deleted;
        }

        public void Update()
        {
            this.State = ObjectState.Updated;
        }

        #region IObjectState Members

        public ObjectState State { get; set; }

        #endregion
    }
}
