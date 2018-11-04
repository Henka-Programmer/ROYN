using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WCFCommon
{
    [DataContract]
    public abstract class ModelBase
    {
        [DataMember]
        public bool Archived { get; set; } = false;

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public int? CreatedByUId { get; set; }

        public ModelBase()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public DateTime UpdatedAt { get; set; }

        [DataMember]
        public int? UpdatedByUId { get; set; }
    }
}