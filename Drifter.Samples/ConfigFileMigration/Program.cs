using System;
using System.IO;
using Newtonsoft.Json.Linq;
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
    2) The config file is json (using Newtonsoft.Json or Json.NET as our Json library)
    3) Our versioning uses a simple integer number
*/

namespace ConfigFileMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var configDataMigrator = new ConfigDataMigrator();
            configDataMigrator.RegisterMigrationStep(
                // Migrate from version 1 to version 2
                new ConfigMigrationStep(1, 2, 
                // The ConfigDataMigrator takes a FileInfo (which is our config file) as the input parameter 
                // for each migration step
                (FileInfo f) => {
                    var path = f.FullName;
                    var json = File.ReadAllText(path);
                    var jsonObject = JObject.Parse(json);

                    // Change our version
                    jsonObject["version"] = 2;

                    // Change "name" key to "id"
                    jsonObject["id"] = jsonObject["name"];
                    jsonObject.Remove("name");

                    // Add "otherkey" to the "context" object
                    ((JObject)jsonObject["context"]).Add("otherkey", "someotherstring");

                    // Fix the typo in the "configTypesSupported" key
                    jsonObject["configTypesSupported"] = jsonObject["configTypesSuported"];
                    jsonObject.Remove("configTypesSuported");

                    // Add more strings to our "configTypesSupported" array
                    ((JArray)jsonObject["configTypesSupported"]).Add("With");
                    ((JArray)jsonObject["configTypesSupported"]).Add("More");
                    ((JArray)jsonObject["configTypesSupported"]).Add("Elements");

                    // Remove "nullable"
                    jsonObject.Remove("nullable");

                    // Add "newField" to root
                    jsonObject.Add("newField", "MyNewString");

                    // Done manipulating our json object, write it back to the file
                    var modifiedJson = jsonObject.ToString();
                    File.WriteAllText(path, modifiedJson);

                    return true;
                })
            );

            var configFileInfo = new FileInfo(@"Drifter.Samples/ConfigFileMigration/Data/premigrationConfig.json");
            var configFileVersion = 1;
            var migrateToVersion = 2;

            // Overriding the output file name so we don't lose the original file for this sample
            configDataMigrator.OutputFileNameOverride = "testrun";
            configDataMigrator.RunMigration(configFileInfo, configFileVersion, migrateToVersion);
        }
    }
}
