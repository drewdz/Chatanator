using PE.Provider.Data.Realm;
using Realms;

namespace Chatanator.Core.Services
{
    public class DataService : DataProvider, IDataService
    {
        #region Constructors

        public DataService()
            : base(new RealmConfiguration
            {
                //  Update the schema version whenever major changes are made to the database
                //  update the version of schema items for revision changes
                SchemaVersion = 0,
                //  TODO: Use the EncryptionKey property to encrypt the entire database.
                ShouldCompactOnLaunch = (totalBytes, usedBytes) =>
                {
                    //  totalBytes refers to the size of the file on disk in bytes (data + free space)
                    //  usedBytes refers to the number of bytes used by data in the file

                    //  Compact if the file is over 100MB in size and less than 50% 'used'
                    //  TODO: These values should be revised as we see the database grow - 100Mb may be too small
                    ulong oneHundredMB = 100 * 1024 * 1024;
                    return (totalBytes > oneHundredMB) && ((double)usedBytes / totalBytes < 0.5);
                }
            })
        {
        }

        #endregion Constructors

        #region Properties

        public DataProvider Provider { get { return this; } }

        #endregion Properties
    }
}
