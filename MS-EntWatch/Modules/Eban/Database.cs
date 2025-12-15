using MS_AnyBaseLib_Shared;

namespace MS_EntWatch.Modules.Eban
{
#pragma warning disable CS8618
    public abstract class Database
    {
        public IAnyBase AnyDB;
        public bool bSuccess = false;
        public bool bDBReady = false;
        public Database(string sDBName, string sDBHost = "", string sDBUser = "", string sDBPassword = "") { }
    }

    public class DB_Mysql : Database
    {
        public DB_Mysql(string sDBName, string sDBHost = "", string sDBUser = "", string sDBPassword = "") : base(sDBName, sDBHost, sDBUser, sDBPassword)
        {
            AnyDB = CAnyBase.Base("mysql");
            AnyDB.Set(MS_AnyBaseLib_Shared.Bases.CommitMode.AutoCommit, sDBName, sDBHost, sDBUser, sDBPassword);
            bSuccess = AnyDB.Init();
        }
    }

    public class DB_PosgreSQL : Database
    {
        public DB_PosgreSQL(string sDBName, string sDBHost = "", string sDBUser = "", string sDBPassword = "") : base(sDBName, sDBHost, sDBUser, sDBPassword)
        {
            AnyDB = CAnyBase.Base("postgre");
            AnyDB.Set(MS_AnyBaseLib_Shared.Bases.CommitMode.AutoCommit, sDBName, sDBHost, sDBUser, sDBPassword);
            AnyDB.Close(); //WTF: Connection already open
            bSuccess = AnyDB.Init();
        }
    }

    public class DB_SQLite : Database
    {
        public DB_SQLite(string sDBName) : base(sDBName)
        {
            AnyDB = CAnyBase.Base("sqlite");
            AnyDB.Set(MS_AnyBaseLib_Shared.Bases.CommitMode.AutoCommit, sDBName);
            bSuccess = AnyDB.Init();
        }
    }

    public class DBConfig
    {
        public string TypeDB { get; set; }
        public string SQLite_File { get; set; }
        public string SQL_Server { get; set; }
        public string SQL_Port { get; set; }
        public string SQL_User { get; set; }
        public string SQL_Password { get; set; }
        public string SQL_NameDatabase { get; set; }
        public DBConfig()
        {
            TypeDB = "sqlite";
            SQLite_File = "database";
        }
    }
#pragma warning restore CS8618
}
