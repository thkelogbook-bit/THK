namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class LctIdentificationAndSafetyMeasuresModel
    {
        /// <summary>
        /// Mã đăng ký lệnh công tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhận diện mối nguy
        /// </summary>
        public string Identification { get; set; }

        /// <summary>
        /// giải pháp an toàn
        /// </summary>
        public string SafetyMeasures { get; set; }
    }
}
