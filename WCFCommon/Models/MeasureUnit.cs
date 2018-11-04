using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WCFCommon
{
    [DataContract]
    [Table("UoMs")]
    public class MeasureUnit : ModelBase
    {
        [DataMember]
        public double Ratio { get; set; }

        [DataMember]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [DataMember]
        public virtual MeasureUnitCategory Category { get; set; }
    }
}