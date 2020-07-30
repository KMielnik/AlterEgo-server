namespace AlterEgo.Core.Settings
{
    public class CoreAnimatorSettings
    {
        public bool IsUsingDocker { get; set; }

        public string ExecPath { get; set; }
        public string DockerImage { get; set; }
        public string PythonStartingPoint { get; set; }

        public string ImagesDirectory { get; set; }
        public string VideosDirectory { get; set; }
        public string TempDirectory { get; set; }
        public string OutputDirectory { get; set; }

        public bool UsingGPU { get; set; }
    }
}
