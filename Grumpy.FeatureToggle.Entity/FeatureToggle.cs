//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Grumpy.FeatureToggle.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class FeatureToggle
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string Feature { get; set; }
        public int Priority { get; set; }
        public bool Enabled { get; set; }
    }
}
