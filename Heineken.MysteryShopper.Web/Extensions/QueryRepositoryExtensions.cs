using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SurveyUs.Web.Extensions
{
    public static class QueryRepositoryExtensions
    {
        private const int _maxlimt = 10;


        private static bool PatchQueryWithCountAndPagination(string query, out string outQuery, int? offset = null, int? limit = null)
        {
            outQuery = query;

            if (string.IsNullOrEmpty(query))
            {
                return false;
            }

            if (offset.HasValue && offset < 0)
            {
                offset = null;
            }

            if (limit.HasValue && limit.Value < 0)
            {
                limit = null;
            }

            // inject count sql
            string countReplacement = "SELECT count(*) over() total_count, ";
            string countCheck = "count";
            string selectPattern = "select";
            string offsetPattern = "offset";
            string limitPattern = "limit";

            string offsetReplacement = " OFFSET @offset ROWS";

            string limitReplacement = " FETCH NEXT @limit ROWS ONLY  ";

            query = query.Trim();

            // if count keyword is in the query, then just return
            if (query.IndexOf(countCheck, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            // if not a select query, then must be update or delete...then just return
            int pos = query.IndexOf(selectPattern, StringComparison.OrdinalIgnoreCase);

            if (pos != 0)
            {
                return false;
            }

            query = query.Substring(0, pos) + countReplacement + query.Substring(pos + selectPattern.Length);

            if (offset.HasValue &&
                query.IndexOf(offsetPattern.Trim(), StringComparison.OrdinalIgnoreCase) < 0)
            {
                query += offsetReplacement;
            }

            if (limit.HasValue &&
                query.IndexOf(limitPattern.Trim(), StringComparison.OrdinalIgnoreCase) < 0)
            {
                query += limitReplacement;
            }

            outQuery = query;
            return true;
        }

        public static async Task<(int, DataTable)> SelectDataTableWithCountAndPagination(SqlConnection connection,
                                                                                  string query, string[] paramName = null,
                                                                                  object[] paramValue = null,
                                                                                  int? offset = 0,
                                                                                  int? limit = _maxlimt)
        {


            if (connection == null ||
                string.IsNullOrEmpty(query))
            {
                return (0, null);
            }

            if (offset < 0)
                offset = 0;

            #region #SL A quick check if number of parameters in query, parameter names and values are the same
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
            var parameterToken = @"@\w+";

            var regex = new Regex(parameterToken, options);
            int parametersCount = regex.Matches(query).Cast<Match>().Select(x => x.Value).Distinct().Count();

            string patchQuery;

            if ((parametersCount > 0) && (paramName != null) && (paramValue != null) &&
                   (parametersCount != paramName.Count()) && (parametersCount != paramValue.Count())
                 )
            {
                return (0, null);
            }
            #endregion

            try
            {
                var isPatch = PatchQueryWithCountAndPagination(query, out patchQuery, offset, limit);

                if (!isPatch)
                {
                    return (0, null);
                }

                using (var cmd = new SqlCommand(patchQuery, connection))
                {
                    if (paramName != null && paramValue != null)
                    {
                        //Verify if the name's count equals the value's count
                        if (paramName.Count() != paramValue.Count())
                        {
                            Debug.WriteLine("ParamName Count != ParamValue Count");
                            return (0, null);
                        }

                        //Add params in the arrays
                        // using parameters will prevent SQL injections attackes
                        for (int i = 0; i < paramName.Count(); i++)
                        {
                            cmd.Parameters.AddWithValue(paramName[i], paramValue[i]);
                        }

                        if (patchQuery.IndexOf("offset") >= 0)
                            cmd.Parameters.AddWithValue("offset", offset);

                        if (patchQuery.IndexOf("limit") >= 0)
                            cmd.Parameters.AddWithValue("limit", limit);
                    }

                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        int total_count = 0;

                        if (dt.Rows != null &&
                            dt.Rows.Count > 0)
                        {
                            var dr = dt.Rows[0];

                            if (dr != null)
                            {
                                total_count = dr["total_count"] != null ? (int)dr["total_count"] : 0;
                            }

                            dt.Columns.Remove("total_count");
                        };

                        await connection.CloseAsync();
                        return (total_count, dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: ---> " + ex.Message);
            }
            return (0, null);
        }
    }
}
