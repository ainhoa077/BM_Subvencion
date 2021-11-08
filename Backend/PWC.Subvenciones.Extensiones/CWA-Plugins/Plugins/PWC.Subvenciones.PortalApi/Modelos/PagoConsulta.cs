namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class PagoConsulta
    {
        public int terminal { get; set; }
        public string order { get; set; }
        public string amount { get; set; }
        public int secure { get; set; }
        public string urlOk { get; set; }
        public string urlKo { get; set; }
        public string currency { get; set; }
    }
}