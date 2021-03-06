//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TechMarket.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_Account()
        {
            this.tbl_Channel = new HashSet<tbl_Channel>();
            this.tbl_Coin = new HashSet<tbl_Coin>();
            this.tbl_Messages = new HashSet<tbl_Messages>();
            this.tbl_Messages1 = new HashSet<tbl_Messages>();
            this.tbl_Shop = new HashSet<tbl_Shop>();
            this.tbl_Evaluete = new HashSet<tbl_Evaluete>();
            this.tbl_Evaluete1 = new HashSet<tbl_Evaluete>();
            this.tbl_Product = new HashSet<tbl_Product>();
        }
    
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string password_v2 { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string facebook { get; set; }
        public string fullname { get; set; }
        public string cmnd { get; set; }
        public Nullable<System.DateTime> brithday { get; set; }
        public string address { get; set; }
        public string images { get; set; }
        public Nullable<int> gender { get; set; }
        public Nullable<int> Role { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Channel> tbl_Channel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Coin> tbl_Coin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Messages> tbl_Messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Messages> tbl_Messages1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Shop> tbl_Shop { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Evaluete> tbl_Evaluete { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Evaluete> tbl_Evaluete1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Product> tbl_Product { get; set; }
    }
}
