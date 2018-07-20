using Realms;

namespace PE.Provider.Data.Realm
{
    public class RealmObjectVersion : RealmObject
    {
        [PrimaryKey]
        public string ObjectName { get; set; }

        public int Revision { get; set; }
    }
}
