using System.Text.Json.Serialization;

namespace Job.Data.Models.Gates
{

    public class GateSetRegistrations : GateSetRegistrationsInput
	{
        [JsonInclude]
        public string Gate = "APPPLUS_SETREGISTRATIONS";
    }

    public class GateSetRegistrationsInput
	{
        [JsonInclude]
        public GateSetRegistrationsInputObject? GateInput { get; set; }
    }

    public class GateSetRegistrationsInputObject
	{
        [JsonInclude]
        public string? Data { get; set; }
    }

    
}
