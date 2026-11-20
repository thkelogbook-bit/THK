namespace Elogbook.Core.Models.BaseCommon
{
    /// <summary>
    /// IFilter
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Prop
        /// </summary>
        string Prop { get; set; }

        /// <summary>
        /// Val
        /// </summary>
        object Val { get; set; }

        /// <summary>
        /// Operator
        /// </summary>
        string Operator { get; set; }
    }

    /// <summary>
    /// Filter
    /// </summary>
    public class Filter : IFilter
    {
        /// <summary>
        /// Prop
        /// </summary>
        public string Prop { get; set; }

        /// <summary>
        /// Val
        /// </summary>
        public object Val { get; set; }

        /// <summary>
        /// Val2
        /// </summary>
        public object Val2 { get; set; }

        /// <summary>
        /// Operator
        /// </summary>
        public string Operator { get; set; }
    }
}
