//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace E_Commerce.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Members
    {
        public Tbl_Members()
        {
            this.Tbl_MemberRole = new HashSet<Tbl_MemberRole>();
        }
    
        public int MemberId { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    
        public virtual ICollection<Tbl_MemberRole> Tbl_MemberRole { get; set; }
    }
}
