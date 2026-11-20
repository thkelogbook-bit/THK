namespace Elogbook.Core.Models.Request.RegistrationTicket
{
    /// <summary>
    /// IdentificationAndSafetyMeasuresModel
    /// </summary>
    public class IdentificationAndSafetyMeasuresModel
    {
        /// <summary>
        /// Mã đăng ký phiếu công tác
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
