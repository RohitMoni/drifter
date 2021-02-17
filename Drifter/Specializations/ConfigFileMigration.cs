using System;
using System.IO;

namespace Drifter.Specializations
{
    public class ConfigMigrationStep : DataMigrationStep<int, FileInfo>
    {
        public ConfigMigrationStep(int from, int to, Func<FileInfo, bool> run, int cost = 1)
            : base(from, to, run, cost) { }
    }

    public class ConfigDataMigrator : DataMigrator<ConfigMigrationStep, int, FileInfo>
    {
        // Modifies a separate file rather than the original before replacing it
        public bool SafeMode { get; set; } = true;

        // Ensures a backup of the original file is maintained so that the migration can be rolled back
        public bool Reversible { get; set; } = true;

        // Overrides the output file name of migrated config file
        public string OutputFileNameOverride { get; set; } = null;

        public override bool RunMigration(FileInfo input, int from, int to)
        {
            configFile = input;

            var migrationPath = GetShortestMigrationPath(from, to);

            outputFile = configFile;
            if (OutputFileNameOverride != null)
            {
                outputFile = new FileInfo(Path.Combine(configFile.DirectoryName, $"{OutputFileNameOverride}.{configFile.Extension}"));
                File.Copy(configFile.FullName, outputFile.FullName);
            }

            for (int i = 0; i < migrationPath.Count; ++i) 
            {
                migrationPath[i].Run(outputFile);
            }

            return true;
        }

        private FileInfo configFile;

        private FileInfo outputFile;
    }
}