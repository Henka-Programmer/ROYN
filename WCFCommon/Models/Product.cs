using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WCFCommon
{
    [DataContract]
    public enum ProductType
    {
        [EnumMember]
        Stockable,

        [EnumMember]
        Consumable,

        [EnumMember]
        Service
    }

    [DataContract]
    [Table("products")]
    public class Product : ModelBase
    {
        [DataMember]
        public bool CanBeExpensed { get; set; }

        [DataMember]
        public bool CanBePurchased { get; set; }

        [DataMember]
        public bool CanBeSold { get; set; }

        [DataMember]
        public virtual ProductCategory Category { get; set; }

        [DataMember]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public virtual MeasureUnit PurchasesUoM { get; set; }

        [DataMember]
        [ForeignKey(nameof(PurchasesUoM))]
        public int PurchasesUoMId { get; set; }

        [DataMember]
        public virtual MeasureUnit SaleUoM { get; set; }

        [DataMember]
        [ForeignKey(nameof(SaleUoM))]
        public int SaleUoMId { get; set; }

        [DataMember]
        public ProductType Type { get; set; }

        [DataMember]
        public double Volum { get; set; }

        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public string Barcode { get; set; }

        [DataMember]
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddYears(10);

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public double MaxQuantity { get; set; }

        [DataMember]
        public double MinQuantity { get; set; }

        [DataMember]
        public bool SoldInPOS { get; set; }
    }
}