namespace Elogbook.Core.Models.BaseCommon
{
    /// <summary>
    /// ISort
    /// </summary>
    public interface ISort
    {
        /// <summary>
        /// Prop
        /// </summary>
        string Prop { get; set; }

        /// <summary>
        /// IsDesc
        /// </summary>
        bool IsDesc { get; set; }
    }

    /// <summary>
    /// Sort
    /// </summary>
    public class Sort : ISort
    {
        /// <summary>
        /// Prop
        /// </summary>
        public string Prop { get; set; }

        /// <summary>
        /// IsDesc
        /// </summary>
        public bool IsDesc { get; set; }
    }
}
