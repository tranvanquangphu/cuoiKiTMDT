//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NhomBoy_Hotel
{
    using System;
    using System.Collections.Generic;
    
    public partial class DanhGia
    {
        public int MaDanhGia { get; set; }
        public Nullable<int> MaDatPhong { get; set; }
        public string NoiDung { get; set; }
        public Nullable<int> SoSao { get; set; }
    
        public virtual DatPhong DatPhong { get; set; }
    }
}
