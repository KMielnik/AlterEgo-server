namespace AlterEgo.Core.Settings
{
    public class CoreAnimatorSettings
    {
        public bool IsUsingDocker { get; set; }

        public string ExecPath { get; set; }
        public string DockerImage { get; set; }
        public string PythonStartingPoint { get; set; }
        public bool UsingGPU { get; set; }
    }
}
