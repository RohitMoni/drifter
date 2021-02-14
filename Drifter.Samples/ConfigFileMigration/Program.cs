using System.IO;
using Drifter.Specializations;

/*  ConfigFileMigration

    This sample is to show off best practices and example usage when working with a config file.
    
    As with all migrations, we're going to go over examples for:
    1) Adding new data
    2) Removing data
    3) Modifying existing data values
    4) Renaming data keys

    Assumptions:
    1) We're working with a single config file
    2) The config file is json
    3) Our versioning uses a simple integer number
*/

namespace ConfigFileMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var configDataMigrator = new ConfigDataMigrator();
            configDataMigrator.RegisterMigrationStep(new ConfigMigrationStep(1, 2, null));

            var configFileInfo = new FileInfo(@"Data/premigrationConfig.json");
            configDataMigrator.RunMigration(configFileInfo);
        }
    }
}
