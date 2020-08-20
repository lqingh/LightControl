using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace DBConn
{
    class DB_SQL
    {
        private string strDBConnectionString = "";
        public MySqlConnection fconConnection;
        private MySqlCommand fcomCommand;
        public MySqlTransaction ftrans;
        public string fDBServerIP = "";
        public string fDBPort = "";
        public string fDBServiceName = "";
        public string fDBAccount = "";
        public string fDBPasswd = "";

        public DB_SQL()//构造函数
        {

        }
        public DB_SQL(string strConnStr)
        {
            strDBConnectionString = strConnStr;
        }

        public MySqlTransaction BeginTransaction(params MySqlCommand[] _cmds)
        {
            ftrans = this.fconConnection.BeginTransaction();//开始事务

            for (int i = 0; i < _cmds.Length; i++)
            {
                if (_cmds[i] != null)
                {
                    _cmds[i].Transaction = ftrans;
                }
            }
            return ftrans;
        }

        public void Commit()
        {
            ftrans.Commit();//提交
            ftrans = null;
        }

        public void Rollback()
        {
            ftrans.Rollback();//回滚
            ftrans = null;
        }

        public MySqlTransaction GetCurrentTransactionTran()
        {
            return ftrans;
        }

        public MySqlCommand CreateCommandInTrans()
        {
            MySqlCommand fcmdCommand = new MySqlCommand("", fconConnection);
            fcmdCommand.Transaction = this.ftrans;

            return fcmdCommand;
        }

        public Boolean DBConnect(ref string _ppsMsg)
        {
            Boolean lbResult;

            if (strDBConnectionString.Trim().Equals(""))
            {
                fconConnection = new MySqlConnection(
                  //    string.Format("server = {0}; User ID={1};Password={2};Initial Catalog={3};Connect Timeout=10",
                  string.Format("server = {0}; user={1};Password={2};database={3};Connect Timeout=10;charset=utf8",
                     this.fDBServerIP, this.fDBAccount, this.fDBPasswd, this.fDBServiceName));//Data Source是数据库地址和端口号，地址和端口号使用,分隔,Initial Catalog是数据库的名字
            }
            else
            {
                fconConnection = new MySqlConnection(strDBConnectionString);
            }
            fcomCommand = new MySqlCommand("", fconConnection);

            if (fconConnection.State != ConnectionState.Open)
            {
                try
                {
                    fconConnection.Open();
                    fcomCommand.CommandTimeout = 100;//资料较大的时候可以调整查询时间
                    lbResult = true;
                }
                catch (System.Exception ex)
                {
                    _ppsMsg = ex.Message.ToString();
                    lbResult = false;
                }
            }
            else
            {
                lbResult = true;
            }
            return lbResult;
        }

        public void DBDisconnect()
        {
            fconConnection.Close();
        }

        public MySqlConnection DBConnecttion
        {
            get { return fconConnection; }
        }

        public void Add_Param(string strName, object objValue)
        {
            fcomCommand.Parameters.AddWithValue(strName, objValue);
        }

        public Boolean ExecuteSQL(string _psSQL, DataTable _ptabDestTable, Boolean _pClearData)
        {
            MySqlDataAdapter Adaptere;

            if (_pClearData) { _ptabDestTable.Clear(); }

            _ptabDestTable.BeginLoadData();

            if (this.ftrans != null) fcomCommand.Transaction = ftrans;

            fcomCommand.CommandText = _psSQL;

            try
            {
                Adaptere = new MySqlDataAdapter(fcomCommand);
                Adaptere.Fill(_ptabDestTable);
                Adaptere.Dispose();
                Adaptere = null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                fcomCommand.Parameters.Clear();
            }

            _ptabDestTable.EndLoadData();

            return true;

        }

        public Boolean ExecuteSQL(string _psSQL, DataTable _ptabDestTable)
        {
            ExecuteSQL(_psSQL, _ptabDestTable, true);
            return false;
        }

        public int ExecuteSQL(string _psSQL)
        {
            DataTable _ptabDestTable = new DataTable();
            ExecuteSQL(_psSQL, _ptabDestTable, true);
            return _ptabDestTable.Rows.Count;
        }

        public int ExecuteDML(string _psDMLSQL)
        {
            int intTmp = 0;
           
            if (this.ftrans != null)
            {
                fcomCommand.Transaction = ftrans;
            }
            fcomCommand.CommandText = _psDMLSQL;
            try
            {
                intTmp = fcomCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                fcomCommand.Parameters.Clear();
            }
            return intTmp;//返回受影响的行数
        }

        public Boolean IsLive()
        {
            MySqlConnection conConnTest;
            MySqlConnectionStringBuilder scbConnTest;

            scbConnTest = new MySqlConnectionStringBuilder();
           
            scbConnTest.Server = this.fDBServerIP;
            scbConnTest.UserID = this.fDBAccount;
            scbConnTest.Password = this.fDBPasswd;
            scbConnTest.Pooling = false;
          
            conConnTest = new MySqlConnection(scbConnTest.ToString());
            try
            {
                if (fconConnection.State == ConnectionState.Closed)
                    return false;
                conConnTest.Open();
                conConnTest.Close();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conConnTest.Dispose();
                conConnTest = null;
                GC.Collect();
            }
        }

        public void CallSp(string strSPName, ref string[] strParam)
        {
            MySqlCommand cmdCallStoreProcedure;
            int i, intOutCount = 0, intInCount = 0;

            if (fconConnection.State != ConnectionState.Open)
                return;

            cmdCallStoreProcedure = new MySqlCommand();
            cmdCallStoreProcedure.Connection = fconConnection;

            if (this.ftrans != null)
            {
                cmdCallStoreProcedure.Transaction = ftrans;
            }

            cmdCallStoreProcedure.CommandType = System.Data.CommandType.StoredProcedure;
            cmdCallStoreProcedure.CommandText = strSPName;

            MySqlCommandBuilder.DeriveParameters(cmdCallStoreProcedure);

            for (i = 0; i < cmdCallStoreProcedure.Parameters.Count; i++)
            {
                if (cmdCallStoreProcedure.Parameters[i].Direction == ParameterDirection.InputOutput)
                {
                    cmdCallStoreProcedure.Parameters[i].Direction = ParameterDirection.Output;
                    intOutCount++;
                }
                else if (cmdCallStoreProcedure.Parameters[i].Direction == ParameterDirection.Output)
                {
                    cmdCallStoreProcedure.Parameters[i].Direction = ParameterDirection.Output;
                    intOutCount++;
                }
                else if (cmdCallStoreProcedure.Parameters[i].Direction == ParameterDirection.Input)
                {
                    if (strParam.Length > intInCount)
                    {
                        cmdCallStoreProcedure.Parameters[i].Value = strParam[intInCount];
                        intInCount++;
                    }
                }
            }

            cmdCallStoreProcedure.ExecuteNonQuery();

            strParam = null;
            strParam = new string[intOutCount];

            intOutCount = 0;

            for (i = 0; i < cmdCallStoreProcedure.Parameters.Count; i++)
            {
                if (cmdCallStoreProcedure.Parameters[i].Direction == ParameterDirection.Output)
                {
                    strParam[intOutCount] = cmdCallStoreProcedure.Parameters[i].Value.ToString();
                    intOutCount++;
                }
            }

            cmdCallStoreProcedure.Dispose();
            cmdCallStoreProcedure = null;
        }
    }
}
