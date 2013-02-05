using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Engine.Core.DB
{
    public class SQLiteDatabase
    {
        String dbConnection;

        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class
        /// </summary>
        public SQLiteDatabase()
        {
            // this seems like it might be extremely slow. not really sure.
            string execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            string fileName = Path.Combine(execPath, "DialoguePrototypeTestDB.s3db").Substring(6);
            dbConnection = String.Format("Data Source={0}; Version=3", fileName);
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SQLiteDatabase(String inputFile)
        {
            dbConnection = String.Format("Data Source={0}", inputFile);
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                var qs = String.Format("UPDATE {0} SET {1} WHERE {2};", tableName, vals, where);
                this.ExecuteNonQuery(qs);
                Console.WriteLine(qs);
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {

                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetDBConnectionPath(String filePath)
        {
            dbConnection = String.Format("Data Source={0}; Version=3", filePath);
        }

        public static string BooleanToDBValue(bool input)
        {
            return input ? "1" : "0";
        }

        public List<NPCPrompt> FindAllPrompts()
        {
            List<NPCPrompt> prompts = new List<NPCPrompt>();
            try
            {
                DataTable entry;
                String query = "select * from Prompt;";
                entry = GetDataTable(query);
                foreach (DataRow r in entry.Rows)
                {
                    prompts.Add(FindPrompt(new Guid((String)r["id"])));
                }
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                //EngineGame.Logger.Error(error);
                prompts.Add(new NPCPrompt());
            }
            return prompts;
        }

        public NPCPrompt FindPrompt(Guid id)
        {
            List<IDialogueAction> promptActions = new List<IDialogueAction>();
            try
            {
                DataTable entry;
                String query = "select speaker \"speaker\", entry \"entry\", ";
                query += "animation \"animation\", sound \"sound\", quest \"quest\", ";
                query += "response_required \"response\" ";
                query += "from Prompt where id = \"" + id.ToString() + "\";";
                entry = GetDataTable(query);
                // only take the first result (there should only be one anyway)
                DataRow result = entry.Rows[0];
                String speaker = (String)result["speaker"];
                String body = (String)result["entry"];

                if (!DBNull.Value.Equals(result["animation"]))
                {
                    promptActions.Add(new AnimationAction((String)result["animation"]));
                }

                if (!DBNull.Value.Equals(result["sound"]))
                {
                    promptActions.Add(new SoundAction((String)result["sound"]));
                }

                if (!DBNull.Value.Equals(result["quest"]))
                {
                    promptActions.Add(new QuestAction((String)result["quest"]));
                }

                Boolean responseRequired = (Boolean)result["response"];

                NPCPrompt prompt = new NPCPrompt(id, speaker, body, promptActions, responseRequired);

                if (responseRequired)
                {
                    prompt.Responses = FindResponses(id);
                }
                return prompt;

            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new NPCPrompt(id, "error", error, promptActions, false);
            }
        }

        public Response FindResponse(Guid id)
        {
            try
            {
                DataTable entry;
                String query = "select entry \"entry\", ";
                query += "next_entry \"next_entry\" ";
                query += "from Response where ID = \"" + id.ToString() + "\";";
                entry = GetDataTable(query);
                // again, there should only be one result
                DataRow result = entry.Rows[0];
                String entryText = (String)result["entry"];
                Guid nextEntry = new Guid((String)result["next_entry"]);
                return new Response(id, entryText, nextEntry);
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new Response(id, "error: " + error, new Guid());
            }
        }

        public List<Response> FindResponses(Guid id)
        {
            List<Response> responses = new List<Response>();
            try
            {
                DataTable entry;
                String query = "select toID \"to\" ";
                query += "from Response_Map where fromID = \"" + id.ToString() + "\";";
                entry = GetDataTable(query);
                foreach (DataRow r in entry.Rows)
                {
                    responses.Add(FindResponse(new Guid((String)r["to"])));
                }
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                responses.Add(new Response(id, "error: " + error, new Guid()));
            }
            return responses;
        }
    }
}

