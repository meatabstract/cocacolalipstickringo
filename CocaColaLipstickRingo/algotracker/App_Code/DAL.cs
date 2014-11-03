/* Original source provided by Lee Davies a.k.a fruitbatinshades.com */
/* used with permission for Remote New Media - All changes after 01/10/2010 are the IP of Remote new media */
/*  (excluding new code provided by Lee Davies from his own libraries after said date) */
using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace Tools.DataAccess
{
	/// <summary>
	/// Wrapper class for paged db procs
	/// </summary>
	public class PagedDbParams
	{
		public PagedDbParams() { }
		public PagedDbParams(Int32 noPerPage, Int32 pageNo) {
			NoPerPage = noPerPage;
			PageNo = pageNo;
		}
		public PagedDbParams(Int32 noPerPage, Int32 pageNo, String orderBy, bool desc)
		{
			NoPerPage = noPerPage;
			PageNo = pageNo;
			OrderBy = orderBy;
			Descending = desc;
		}

		private String _OrderBy = "Name";
        /// <summary>
        /// The string passed into the stored proc for ordering
        /// </summary>
		public String OrderBy
		{
			get { return _OrderBy; }
			set { _OrderBy = value; }
		}
        /// <summary>
        /// No of items per page
        /// </summary>
		public Int32 NoPerPage { get; set; }
        /// <summary>
        /// Current page no
        /// </summary>
		public Int32 PageNo { get; set; }
        /// <summary>
        /// Whether order is descending or not
        /// </summary>
		public bool Descending { get; set; }

        /// <summary>
        /// Get the params for the stored proc
        /// </summary>
        /// <returns></returns>
		public List<DbParameter> GetParams()
		{
			DAL Dal = Utils.GetDAL();
			DAL.Parameters pa = new DAL.Parameters(Dal.ProviderFactory);
			pa.Add("@PageNo", PageNo);
			pa.Add("@NoOff", NoPerPage);
			pa.Add("@OrderBy", !String.IsNullOrEmpty(OrderBy) ? OrderBy : null);
			pa.Add("@Desc", Descending ? 1 : 0);

			return pa.Params;
		}
	}
	public static class Utils
	{
        /// <summary>
        /// Get the default DAL (DBConnection in web.config)
        /// </summary>
        /// <returns></returns>
		public static DAL GetDAL()
		{
			
			//string DBConn = ConfigurationManager.AppSettings["DBConnection"];
			//string Provider = ConfigurationManager.AppSettings["ProviderFactory"];
			string DBConn, Provider;
			//try
			//{
				DBConn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
				Provider = ConfigurationManager.ConnectionStrings["DBConnection"].ProviderName;
			//}
			//catch (Exception)
			//{
			//	throw new Exception("Cannot find ConnectionString DBConnection! Please add to your config file.");
			//}
			
			try
			{
				DAL dal = new DAL(DBConn, Provider);
				return dal;
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
		}
        /// <summary>
        /// Get a custom DAL with your own provider and connection string
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
		public static DAL GetDAL(string dbConn, string provider)
		{
			try
			{
				DAL dal = new DAL(dbConn, provider);
				return dal;
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
		}

	}

			
    ///// <summary>
    ///// Passed back from some db access routines
    ///// </summary>
    //public struct DbActionInfo
    //{
    //    public DbActionInfo(int Result, eDbActionInfo ActionInfo)
    //    {
    //        ResultInt = Result;
    //        ResultString=string.Empty;
    //        Action = ActionInfo;
    //    }
    //    public DbActionInfo(string Result, eDbActionInfo ActionInfo)
    //    {
    //        ResultInt = int.MinValue;
    //        ResultString=Result;
    //        Action = ActionInfo;
    //    }
    //    public int ResultInt;
    //    public string ResultString;
    //    public eDbActionInfo Action;
    //}
    ///// <summary>Action that the db access routine carried out.</summary>
    //public enum eDbActionInfo
    //{
    //    Added,
    //    AlreadyExists,
    //    Deleted,
    //    Updated,
    //    NoChange,
    //    NotFound,
    //    Saving,
    //    Error
    //}
    //public class DbPropertySet
    //{
    //    private String _Command = String.Empty;
    //    public String Command
    //    {
    //        get { return _Command; }
    //        set { _Command = value; }
    //    }
    //    private CommandType _Type = CommandType.StoredProcedure;
    //    public CommandType Type
    //    {
    //        get { return _Type; }
    //        set { _Type = value; }
    //    }

    //    private RNMCore.DataAccess.DAL.Parameters _Params = null;
    //    public RNMCore.DataAccess.DAL.Parameters Params
    //    {
    //        get { return _Params; }
    //        set { _Params = value; }
    //    }
    //}
	/// <summary>
	/// Description of DAL
	/// </summary>
	public class DAL
	{
		public DAL()
		{
		}
		
		public DAL(string connectionString, DbProviderFactory provIderFactory)
		{
			ProviderFactory = provIderFactory;
			ConnectionString = connectionString;
		}
		public DAL(string connectionString, string ProviderFactoryName)
		{
			ProviderFactory = System.Data.Common.DbProviderFactories.GetFactory(ProviderFactoryName);
			ConnectionString = connectionString;
		}
		public CommandBehavior ReaderCommandBehavior = CommandBehavior.CloseConnection;
		public DbProviderFactory ProviderFactory;
		public string ConnectionString;
		public int CommandTimeout;
		private DbTransaction _trans;
		private IsolationLevel _isolationLevel;
		private DbConnection _conn;
		private Int32 _cmdTimeout;

		public List<DbConnection> Connections = new List<DbConnection>();
		
		private void PrepareAll(ref DbCommand cmd,ref DbConnection conn,string commandText, CommandType cmdType , Parameters Params)
		{
			cmd.CommandText = commandText;
			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = cmdType;
			cmd.Connection = conn;
			conn.StateChange += new StateChangeEventHandler(conn_StateChange);

			//if (!Connections.Contains(conn))
			//{
			//    Connections.Add(conn);
			//}

			if (Params!=null){
				foreach (DbParameter p in Params)
				{
					cmd.Parameters.Add(p);
				}
			}
			
			if (IsInTransaction()){
				cmd.Transaction = _trans;
				cmd.Connection = _conn;
			}else{
				conn.ConnectionString = ConnectionString;
				conn.Open();
			}
		}

		void conn_StateChange(object sender, StateChangeEventArgs e)
		{
			if (e.CurrentState == ConnectionState.Closed)
			{
				if (sender is DbConnection)
				{
					DbConnection conn = sender as DbConnection;
					conn.Close();
					conn.Dispose();
					conn = null;
				}
			}
		}

#region "ExecDataReader"
        /// <summary>
        /// Execute the sql/stored proc and return a DbDataReader object
        /// </summary>
        /// <param name="strSQL">Sql text or command name</param>
        /// <param name="cmdtype">Stored Proc, Sql Text or Table</param>
        /// <param name="Params">Collection of parameters</param>
        /// <param name="clearParams">Clear the params so you reuse the connection</param>
        /// <returns></returns>
		public DbDataReader ExecDataReader(string strSQL, CommandType cmdtype, Parameters Params, bool clearParams)
		{

			DbConnection conn = ProviderFactory.CreateConnection();
			DbCommand cmd = ProviderFactory.CreateCommand();
			try {
	
				PrepareAll(ref cmd, ref conn, strSQL, cmdtype, Params);
				return cmd.ExecuteReader(ReaderCommandBehavior);
			}
	
			catch (Exception ex) {
				if (!IsInTransaction()) {
					conn.Close();
					conn.Dispose();
				}
				//GenericExceptionHandler(ex);
				throw ex;
			}
			finally {
				//clear params so we can reuse them FBIS:Testing this, 17/07/2008
				if (clearParams) cmd.Parameters.Clear();
				cmd.Dispose();
				//Connections.Add(conn);
			}
	
		}

		public DbDataReader ExecDataReader(string strSQL, CommandType cmdtype, Parameters Params)
		{
			return ExecDataReader(strSQL, cmdtype, Params, true);
		}
		public DbDataReader ExecDataReader(string strSQL, CommandType cmdtype)
		{
			return ExecDataReader(strSQL, cmdtype, null, true);
		}
		/// <summary>
		/// Utility to call a proc with @UniqueId
		/// </summary>
		/// <param name="uniqueId">id to use</param>
		/// <param name="strSQL">Stored proc name</param>
		/// <returns></returns>
		public DbDataReader ExecDataReader(string strSQL, int uniqueId)
		{
			DAL.Parameters pa = new DAL.Parameters(ProviderFactory);
			pa.Add("@UniqueId", uniqueId);
			return ExecDataReader(strSQL, CommandType.StoredProcedure, pa);
		}
#endregion
#region "ExecNonQuery"

		public int ExecNonQuery(string strSQL, CommandType cmdType)
		{
			return ExecNonQuery(strSQL, cmdType, null);
		}
	/// <summary>
	/// Execute a stored proc or sql but return nothing
	/// </summary>
	/// <param name="strSQL">Sql text or Proc Name</param>
        /// <param name="cmdType">Stored Proc, Sql Text or Table</param>
	/// <param name="Params">Collection of parameters</param>
	/// <returns>No Off Rows Affected</returns>
		public int ExecNonQuery(string strSQL, CommandType cmdType, Parameters Params)
		{
	
			DbCommand cmd = ProviderFactory.CreateCommand();
			DbConnection conn = ProviderFactory.CreateConnection();
			try {
				PrepareAll(ref cmd, ref conn, strSQL, cmdType, Params);
				return cmd.ExecuteNonQuery();
			}
	
			catch (Exception ex) {
				//GenericExceptionHandler(ex);
				throw ex;
			}
			finally {
				//clear params so we can reuse them FBIS:Testing this, 17/07/2008
				cmd.Parameters.Clear();
				if (!IsInTransaction()) {
					conn.Close();
					conn.Dispose();
				}
				cmd.Dispose();
			}
		}


#endregion
#region "ExecScalar"
	/// <summary>
	/// Execute sql or stored proc and return the single value
	/// </summary>
    /// <param name="strSQL">Sql text or Proc Name</param>
    /// <param name="cmdType">Stored Proc, Sql Text or Table</param>
    /// <param name="Params">Collection of parameters</param>
	/// <returns>The single value returned from the db</returns>
		public object ExecScalar(string strSQL, CommandType cmdtype, Parameters Params)
		{
	
			DbConnection conn = ProviderFactory.CreateConnection();
			DbCommand cmd = ProviderFactory.CreateCommand();
			try {
	
				PrepareAll(ref cmd, ref conn, strSQL, cmdtype, Params);
				return cmd.ExecuteScalar();
			}
			catch (Exception ex) {
				//GenericExceptionHandler(ex);
				//System.Windows.Forms.MessageBox.Show(string.Format("{0} connections are open!",Connections.Count));
				throw ex;
			}
			finally {
				//clear params so we can reuse them FBIS:Testing this, 17/07/2008
				cmd.Parameters.Clear();
				
				if (!IsInTransaction()) {
					conn.Close();
					conn.Dispose();
				}
				cmd.Dispose();
			}
			
		}
	
		public object ExecScalar(string strSQL, CommandType cmdtype)
		{
			return ExecScalar(strSQL, cmdtype, null);
		}
#endregion
#region "ExecDataSet"
	public void ExecDataSet(DataSet ds, string strSQL, CommandType cmdtype)
	{
		ExecDataSet(ds, strSQL, cmdtype, null);
	}

	public DataSet ExecDataSet(string strSQL, CommandType cmdtype)
	{
		return ExecDataSet(strSQL, cmdtype, null);
	}

	public DataSet ExecDataSet(string strSQL, CommandType cmdtype, Parameters Params)
	{

		DataSet ds = new DataSet("DataSet");
		ExecDataSet(ds, strSQL, cmdtype, Params);
		return ds;
	}

	public void ExecDataSet(DataSet ds, string strSQL, CommandType cmdtype, Parameters Params)
	{
		DbDataAdapter da = ProviderFactory.CreateDataAdapter();
		DbCommand cmd = ProviderFactory.CreateCommand();
		DbConnection conn = ProviderFactory.CreateConnection();
		try {
			da = ProviderFactory.CreateDataAdapter();
			PrepareAll(ref cmd, ref conn, strSQL, cmdtype, Params);
			da.SelectCommand = cmd;
			da.Fill(ds);
		}

		catch (Exception ex) {
			//GenericExceptionHandler(ex);
			throw ex;
		}
		finally {
			//clear params so we can reuse them FBIS:Testing this, 17/07/2008
			cmd.Parameters.Clear();
			if (!IsInTransaction()) {
				conn.Close();
				conn.Dispose();
			}
			cmd.Dispose();
			((IDisposable)da).Dispose();
		}


	}


	#endregion
	
		#region "Transactions"
	public void BeginTrans(string connString, IsolationLevel transisolationLevel)
	{
		_conn = ProviderFactory.CreateConnection();
		_conn.ConnectionString = connString;
		_conn.Open();
		_trans = _conn.BeginTransaction(transisolationLevel);
	}

	public void BeginTrans(IsolationLevel transisolationLevel)
	{
		_conn = ProviderFactory.CreateConnection();
		_conn.ConnectionString = ConnectionString;
		_conn.Open();
		_trans = _conn.BeginTransaction(transisolationLevel);
	}

	public void CommitTrans()
	{
		CommitTrans(true);
	}

	// This is for DataReader usage only. The caller has to pass false here so that
	// the connection is not closed before the DR is closed
	public void CommitTrans(bool CloseConnection)
	{
		_trans.Commit();
		DisposeTrans(CloseConnection);
	}

	public void AbortTrans()
	{
		if (IsInTransaction()) {
			_trans.Rollback();
			DisposeTrans(true);
		}
	}

	private void DisposeTrans(bool CloseConnection)
	{
		if (CloseConnection) {
			if ((_conn != null)) {
				_conn.Close();
				_conn.Dispose();
			}
		}
		_trans.Dispose();
	}

	public bool IsInTransaction()
	{
		return (_trans != null);
	}

	public void Dispose()
	{
		if ((_conn != null)) {
			if (_conn.State != ConnectionState.Closed) {
				_conn.Close();
			}
			_conn.Dispose();
			_conn = null;
		}
		if (Connections.Count > 0)
		{
			while (Connections.Count > 0)
			{
				DbConnection c = Connections[0];
				c.Close();
				c.Dispose();
				Connections.Remove(c);
			}
		}
		// Unregister object for finalization.
		GC.SuppressFinalize(this);
	}
	#endregion
	/// <summary>
	/// Collection used to create parameters to pass to stored procedures
	/// </summary>
		public class Parameters : ICollection
		{
			public System.Collections.Generic.List<DbParameter> Params = new List<DbParameter>();
			private DbProviderFactory _Factory;
			public Parameters(DbProviderFactory Factory)
			{
				_Factory = Factory;
			}
			public void Add(DbParameter param)
			{
				Params.Add(param);
			}
			public void Add(string parameterName, object value)
			{
				DbParameter p = _Factory.CreateParameter();
				p.ParameterName=parameterName;
				p.Value=value;
				p.Direction= ParameterDirection.Input;
				Params.Add(p);
			}
			
			public void Add(string parameterName, object value, bool ForceNull)
			{
				DbParameter p = _Factory.CreateParameter();
				p.ParameterName=parameterName;
				//Force null when string is empty
				if (ForceNull && (value == null || value.ToString().Trim()==""))
					value = DBNull.Value;
				
				p.Value=value;
				p.Direction= ParameterDirection.Input;
				Params.Add(p);
			}

			public DbParameter Add(string parameterName, object value, ParameterDirection direction)
			{
				DbParameter p = _Factory.CreateParameter();
				p.ParameterName=parameterName;
				p.Value=value;
				p.Direction= direction;
				Params.Add(p);
				//return the parameter to save iterating later
				return p;
			}
			public int Count {
				get {
					return Params.Count;
				}
			}
			public void Clear(){
				Params.Clear();
			}
			public object SyncRoot {
				get {
					throw new NotImplementedException();
				}
			}
			
			public bool IsSynchronized {
				get {
					throw new NotImplementedException();
				}
			}
			
			public void CopyTo(Array array, int index)
			{
				Params.CopyTo((DbParameter[])array,index);
			}
			
			public IEnumerator GetEnumerator()
			{
				return Params.GetEnumerator();
			}
			/// <summary>
			/// Converts a Date variable to its SQL compatable String Version
			/// </summary>
			/// <param name="TheDate" type="Date">
			/// <para>
			/// Date to convert
			/// </para>
			/// </param>
			/// <returns>
			/// A SQL Safe DateTime string.
			/// </returns>
			public string DateToSQLFormat(System.DateTime TheDate)
			{
				string str = null;
				{
					str = (TheDate.Year.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + TheDate.Month.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + TheDate.Day.ToString().PadLeft(2, Convert.ToChar("0")) + " " + TheDate.Hour.ToString().PadLeft(2, Convert.ToChar("0")) + ":" + TheDate.Minute.ToString().PadLeft(2, Convert.ToChar("0")) + ":" + TheDate.Second.ToString().PadLeft(2, Convert.ToChar("0")) + ":" + TheDate.Millisecond.ToString().PadLeft(2, Convert.ToChar("0")));
				}
				return str;
			}
		
		}
		/// <summary>
		/// Helper routine to create parameters outside the collection if required
		/// </summary>
		/// <param name="dal">DAL to use</param>
		/// <param name="parameterName">Name of parameter</param>
		/// <param name="value">The value</param>
		/// <param name="direction">Whether its standard, Input, output or both</param>
		/// <returns>newly created parameter</returns>
        public DbParameter CreateParam(DAL dal, string parameterName, object value, ParameterDirection direction)
		{
			DbParameter p = dal.ProviderFactory.CreateParameter();
			p.ParameterName = parameterName;
			p.Value = value;
			p.Direction = direction;
			return p;
		}
        /// <summary>
        /// Stops null string errors
        /// </summary>
        /// <param name="o">DB field</param>
        /// <returns>return the string in not DBNull otherwise String.Empty</returns>
		public string NoNull_String(object o)
		{
			if (o != DBNull.Value && o != null)
			{
				return o.ToString();
			}else{
				return string.Empty;
			}
		}
        /// <summary>
        /// Stops null number erros
        /// </summary>
        /// <param name="o">db field</param>
        /// <returns>The value as a double if not null or 0</returns>
		public double NoNull_Number(object o)
		{
			if (o != DBNull.Value)
			{
				return Convert.ToDouble(o);
			}else{
				return new Double();
			}
		}
        /// <summary>
        /// Stops null decimal errors
        /// </summary>
        /// <param name="o">DB field</param>
        /// <returns>The value if not null or Decimal.MinusOne</returns>
		public decimal NoNull_Decimal(object o)
		{
			if (o != DBNull.Value)
			{
				return Convert.ToDecimal(o);
			}
			else
			{
				return Decimal.MinusOne;
			}
		}
        /// <summary>
        /// Stops null int errors
        /// </summary>
        /// <param name="o">DB field</param>
        /// <returns>The value if not null or 0</returns>
		public Int32 NoNull_Int(object o)
		{
			if (o != DBNull.Value)
			{
				return Convert.ToInt32(o);
			}
			else
			{
				return new Int32();
			}
		}
        /// <summary>
        /// Stops null date errors
        /// </summary>
        /// <param name="o">DB field</param>
        /// <returns>The value if not null or DateTime.MinValue</returns>
		public DateTime NoNull_Date(object o)
		{
			if (o != DBNull.Value)
			{
				return Convert.ToDateTime(o);
			}
			return DateTime.MinValue;
		}
        /// <summary>
        /// Stops null guid errors
        /// </summary>
        /// <param name="o">DB field</param>
        /// <returns>The value if not null or Guid.Empty</returns>
		public Guid NoNull_Guid(object o)
		{
			if (o != DBNull.Value)
			{
				return new Guid(o.ToString());
			}
			return Guid.Empty;
		}
        /// <summary>
        /// Converts DBNull to null
        /// </summary>
        /// <param name="o">DB field</param>
        /// <returns>Return string if possible else null</returns>
		public string DBNullAsNull(object o)
		{
			if (o == DBNull.Value)
			{
				return null;
			}
			return o.ToString();
		}
		/// <summary>
		/// Returns null if an empty string is passed in
		/// </summary>
		/// <param name="value">String</param>
		/// <returns>DBNull if string is empty or null</returns>
		public object EmptyStringAsDBNull(string value)
		{
			if (value==null || value=="") 
				{return DBNull.Value ;}
			return value;
		}
        /// <summary>
        /// Returns null if Date is not set
        /// </summary>
        /// <param name="value">Date</param>
        /// <returns>DBNull if date is DateTime.MinValue or DateTime.MaxValue</returns>
		public object EmptyDateAsDBNull(DateTime value)
		{
			if (value == DateTime.MinValue || value == DateTime.MaxValue)
			{ return DBNull.Value; }
			return value;
		}
        /// <summary>
        /// Converts empty guid to DBNull
        /// </summary>
        /// <param name="value">Guid</param>
        /// <returns>DBNull if Guid.Empty</returns>
		public object EmptyGuidAsDBNull(Guid value)
		{
			if (value == Guid.Empty)
			{ return DBNull.Value; }
			return value;
		}
        /// <summary>
        /// returns a null if value is 0
        /// </summary>
        /// <param name="value">int</param>
        /// <returns>DBNull if value = 0</returns>
		public object ZeroAsDBNull(int value)
		{
			if (value == 0)
			{
				return DBNull.Value;
			}
			else
			{
				return value;
			}
		}
        /// <summary>
        /// Return a null if value is 0
        /// </summary>
        /// <param name="value">decimal</param>
        /// <returns>DBNull if value=0</returns>
        public object ZeroAsDBNull(decimal value)
        {
            if (value == decimal.MinusOne)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// Return a null if value is 0
        /// </summary>
        /// <param name="value">decimal</param>
        /// <returns>DBNull if value=0</returns>
        public object ZeroAsDBNull(decimal value, bool actuallyZero)
        {
            if (actuallyZero)
            {
                if (value == decimal.Zero)
                {
                    return DBNull.Value;
                }
                else
                {
                    return value;
                }
            }
            else
                return ZeroAsDBNull(value);
        }
        /// <summary>
        /// Closes and disposes datareaders
        /// </summary>
        /// <param name="dr"></param>
		public void TidyUp(DbDataReader dr){
			if (!dr.IsClosed) dr.Close();
			dr.Dispose();
		}
        /// <summary>
        /// Test the connection string
        /// </summary>
        /// <returns>true if the connection succeeds</returns>
		public bool TestConnection()
		{
			try
			{
				DbConnection conn = ProviderFactory.CreateConnection();
				conn.ConnectionString = ConnectionString;
				conn.Open();
				conn.Close();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
