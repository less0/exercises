using System.Diagnostics;

namespace bowling_backend_persistence_tests
{
    public class DockerDatabaseFixture : IDisposable
    {
        public DockerDatabaseFixture()
        {
            Console.WriteLine("Spinning up SQL Express container.");
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "docker",
                Arguments = @$"run -e ""ACCEPT_EULA=Y"" -e ""MSSQL_SA_PASSWORD={Constants.SqlPassword}"" -e ""MSSQL_PID=Express"" -p 63000:1433 -d --name bowling_sql_server mcr.microsoft.com/mssql/server:2019-latest",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            });
            process.WaitForExit();
            if(process.ExitCode != 0)
            {
                throw new Exception("Failed to run SQL Express");
            }
        }

        public void Dispose()
        {
            var removeProcess = Process.Start("docker", "rm --force bowling_sql_server");
            removeProcess.WaitForExit();
        }
    }
}