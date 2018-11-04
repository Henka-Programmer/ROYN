using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WCFCommon
{
    [DataContract]
    [Table("productCategories")]
    public class ProductCategory : ModelBase
    {
        [DataMember]
        [ForeignKey(nameof(ParentCategory))]
        public int? ParentCategoryId { get; set; }

        [DataMember]
        public virtual ProductCategory ParentCategory { get; set; }
    }
}