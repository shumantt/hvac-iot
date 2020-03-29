namespace ServiceLayerApi.CommandProcessing.Models
{
    public class DecisionCommandProcessResult
    {
        public ParameterCommandProcessResult[] ParameterCommandProcessResults { get; set; }
    }
    
    public class ParameterCommandProcessResult
    {
        public double Impact { get; set; }
        public bool Failed { get; set; }
        public string Error { get; set; }   
    }
}