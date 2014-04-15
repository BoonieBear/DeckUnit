// ***********************************************************************
// Assembly         : BoonieBear.DeckUnit.DAL
// Author           : Fuxiang
// Created          : 12-16-2013
//
// Last Modified By : Fuxiang
// Last Modified On : 12-27-2013
// ***********************************************************************
// <copyright file="SqliteHelperSQL.cs" company="BoonieBear">
//     Copyright (c) BoonieBear. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;

namespace BoonieBear.DeckUnit.DAL.SqliteDAL
{
    /// <summary>
    /// Sqlite数据库操作类
    /// </summary>
    public class SqliteHelperSQL
    {

        /// <summary>
        /// 声明一个连接对象
        /// </summary>
        private SQLiteConnection  _dbConnection;

        /// <summary>
        /// 声明一个操作数据库命令
        /// </summary>
        private SQLiteCommand _dbCommand;

        /// <summary>
        /// 声明一个读取结果集的一个或多个结果流
        /// </summary>
        private SQLiteDataReader _reader;

        public bool Linked { get; set; }

        /// <summary>
        /// 数据库的连接字符串，用于建立与特定数据源的连接
        /// </summary>
        /// <param name="connectionString">数据库的连接字符串，用于建立与特定数据源的连接</param>
        public SqliteHelperSQL(string connectionString)
        {
            Linked = OpenDB(connectionString);
            Debug.WriteLine(connectionString);
        }

        public bool OpenDB(string connectionString)
        {
            try
            {
                _dbConnection = new SQLiteConnection(connectionString);
                _dbConnection.Open();
                Linked = true;
                Debug.WriteLine("Connected to db");
                
            }
            catch (Exception e)
            {
                string temp1 = e.ToString();
                Linked = false;
                Debug.WriteLine(temp1);
            }
            return Linked;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseSqlConnection()
        {
            if (_dbCommand != null)
            {
                _dbCommand.Dispose();
            }
            _dbCommand = null;
            if (_reader != null)
            {
                _reader.Dispose();
            }
            _reader = null;
            if (_dbConnection != null)
            {
                _dbConnection.Close();
            }
            _dbConnection = null;
            Debug.WriteLine("Disconnected from db.");
        }

        /// <summary>
        /// 执行查询sqlite语句操作
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public SQLiteDataReader ExecuteQuery(string sqlQuery)
        {
            _dbCommand = _dbConnection.CreateCommand();
            
            _dbCommand.CommandText = sqlQuery;
            Debug.WriteLine(sqlQuery);
            _reader = _dbCommand.ExecuteReader();
           
            return _reader; 
            
        }

        /// <summary>
        /// 查询该表所有数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public SQLiteDataReader ReadFullTable(string tableName)
        {
            string query = "SELECT * FROM " + tableName;
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 动态添加表字段到指定表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="values">字段集合</param>
        /// <returns></returns>
        public SQLiteDataReader InsertInto(string tableName, string[] values)
        {
            string query = "INSERT INTO " + tableName + " VALUES (" + "\"" + values[0] + "\"";
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + "\""+values[i]+"\"";
            }
            query += ")";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 动态更新表结构
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">字段集</param>
        /// <param name="colsvalues">对于集合值</param>
        /// <param name="selectkey">要查询的字段</param>
        /// <param name="selectvalue">要查询的字段值</param>
        /// <returns></returns>
        public SQLiteDataReader UpdateInto(string tableName, string[] cols,
            string[] colsvalues, string selectkey, string selectvalue)
        {
            string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + "\""+colsvalues[0]+"\"";
            for (int i = 1; i < colsvalues.Length; ++i)
            {
                query += ", " + cols[i] + " =" + "\"" + colsvalues[i] + "\"";
            }
            if (selectkey.Trim()!="")
                query += " WHERE " + selectkey + " = " + "\"" + selectvalue + "\"" + " ";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 动态删除指定表字段数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">字段</param>
        /// <param name="colsvalues">字段值</param>
        /// <returns></returns>
        public SQLiteDataReader Delete(string tableName, string[] cols, string[] colsvalues)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + "\"" + colsvalues[0] + "\"";
            for (int i = 1; i < colsvalues.Length; ++i)
            {
                query += " or " + cols[i] + " = " + "\"" + colsvalues[i] + "\"";
            }
            
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 动态添加数据到指定表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">字段</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public SQLiteDataReader InsertIntoSpecific(string tableName, string[] cols,
            string[] values)
        {
            if (cols.Length != values.Length)
            {
                throw new SQLiteException("columns.Length != values.Length");
            }
            string query = "INSERT INTO " + tableName + "(" + cols[0];
            for (int i = 1; i < cols.Length; ++i)
            {
                query += ", " + cols[0];
            }
            query += ") VALUES (" +"\"" +values[0]+"\"" ;
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + "\"" +values[i]+"\"" ;
            }
            query += ")";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 动态删除表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public SQLiteDataReader DeleteContents(string tableName)
        {
            string query = "DROP TABLE " + tableName;
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 动态创建表
        /// </summary>
        /// <param name="name">表名</param>
        /// <param name="col">字段</param>
        /// <param name="colType">类型</param>
        /// <returns></returns>
        public SQLiteDataReader CreateTable(string name, string[] col, string[] colType)
        {
            if (col.Length != colType.Length)
            {
                throw new SQLiteException("columns.Length != colType.Length");
            }
            string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];
            for (int i = 1; i < col.Length; ++i)
            {
                query += ", " + col[i] + " " + colType[i];
            }
            query += ")";
            Debug.WriteLine(query);
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 根据查询条件 动态查询数据信息
        /// </summary>
        /// <param name="tableName">表</param>
        /// <param name="items">查询数据集合</param>
        /// <param name="col">字段</param>
        /// <param name="operation">操作</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public SQLiteDataReader SelectWhere(string tableName, string[] items, string[] col, string[] operation,
            string[] values)
        {
            if (col.Length != operation.Length || operation.Length != values.Length)
            {
                throw new SQLiteException("col.Length != operation.Length != values.Length");
            }
            string query = "SELECT " + items[0];
            for (int i = 1; i < items.Length; ++i)
            {
                query += ", " + items[i];
            }
            query += " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";
            for (int i = 1; i < col.Length; ++i)
            {
                query += " AND " + col[i] + operation[i] + "'" + values[i] + "' ";
            }
            return ExecuteQuery(query);
        }

        

        /// <summary>
        /// 根据字符条件查找记录
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public SQLiteDataReader SelectWhere(string tableName, string strWhere)
        {
            var querystring = new StringBuilder("SELECT * FROM " + tableName);
            if (strWhere.Trim() != "")
                querystring.Append(" where " + strWhere);
            return ExecuteQuery(querystring.ToString());
        }
        /// <summary>
        /// 确定某项数值是否已经存在，用来判断数据重复
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="col"></param>
        /// <param name="compareitem"></param>
        /// <returns></returns>
        public bool CheckExistVal(string tableName, string col, string compareitem)
        {
            string query = "SELECT " + col + " FROM " + tableName;
            var reader = ExecuteQuery(query);
            while (reader.Read())
            {
                if(compareitem==reader.GetValue(0).ToString())
                    return true;
                
            }
            return false;
        }
    }
}