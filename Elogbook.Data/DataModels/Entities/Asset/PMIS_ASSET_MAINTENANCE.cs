namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_ASSET_MAINTENANCE")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_ASSET_MAINTENANCE : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
		public string AMID { get; set; }

		public string WTYPEID { get; set; }

		public DateTime? TACFINISHDATE { get; set; }

		public DateTime? TACSTARTDATE { get; set; }

		public DateTime? TPLFINISHDATE { get; set; }

		public DateTime? TPLSTARTDATE { get; set; }

		public string TASKDESC { get; set; }

		public string TASKID { get; set; }

		public string WORDERID { get; set; }

		public string ASSETID { get; set; }

		public int? JobId { get; set; }

		public bool? IsSync { get; set; }

		public int? Status { get; set; }
	}
}
