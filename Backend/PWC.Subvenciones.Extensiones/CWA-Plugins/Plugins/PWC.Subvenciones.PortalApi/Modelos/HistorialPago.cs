namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class HistorialPago
    {
        public int terminal { get; set; }
        public string order { get; set; }
        public string amount { get; set; }
        public string amountDisplay { get; set; }
        public string errorCode { get; set; }
        public string errorDescription { get; set; }
        public string feeEuro { get; set; }
        public string currency { get; set; }
        public string feePercent { get; set; }
        public string methodId { get; set; }
        public int operationId { get; set; }
        public string operationName { get; set; }
        public string originalIp { get; set; }
        public string pan { get; set; }
        public string response { get; set; }
        public int state { get; set; }
        public string stateName { get; set; }
        public string timestamp { get; set; }
    }
}